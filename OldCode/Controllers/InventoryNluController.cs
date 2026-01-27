// using System.Net.Http.Json;
using FunTimePIE;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using System.Text.Json;
using YYTools;
using static FunTimePIE.Controllers.InventoryController;

[ApiController]
[Route("api/nlu")]
public class InventoryNluController : ControllerBase
{
    private readonly HttpClient _http;
    private readonly IConfiguration _cfg;
    //private readonly IHttpClientFactory _httpClientFactory;

    public record NluRequest(string text);
    public record InventoryNluResult(bool isInventoryCheck, string? itemCode, string? normalizedQuery);

    public InventoryNluController(IHttpClientFactory httpFactory, IConfiguration cfg)
    {
        _http = httpFactory.CreateClient();
        _cfg = cfg;
    }

    [HttpPost("inventory")]
    public async Task<ActionResult<InventoryCheckResponse>> Inventory([FromBody] NluRequest req)
    {
        var apiKey = _cfg["OpenAI:ApiKey"];
        InventoryNluResult GPTResult = null;
        InventoryCheckResponse R= new InventoryCheckResponse();
        string msg = "";
        if (string.IsNullOrWhiteSpace(apiKey))
        //return Problem("Missing OpenAI API key.");
        {
            R.Status = -1;
            R.Message = "Missing OpenAI API key.";
            return Created("",R);
        }

        DBTool mydbt1 = new DBTool(Utility.DefaultConnectionString);

        mydbt1.AddParam("Query", "Inventory");
        mydbt1.AddParam("Request", req.text);

        DataSet DS1 = mydbt1.RunCMD("psp_GPT_Prompts");

        var system = DS1.Tables[0].Rows[0]["system_text"].ToString();
        var user = DS1.Tables[0].Rows[0]["user_text"].ToString();
        var GPTmodel = DS1.Tables[0].Rows[0]["GPTmodel"].ToString();
        var GPTtemperature = DS1.Tables[0].Rows[0]["temperature"].ToString();

        var payload = new
        {
            model = GPTmodel,// "gpt-4.1-mini",
            messages = new object[]
            {
                new { role = "system", content = system },
                new { role = "user", content = user }
            },
            temperature = int.Parse(GPTtemperature)
        };

        var httpReq = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        httpReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        httpReq.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var resp = await _http.SendAsync(httpReq);
        var json = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode) return Problem($"OpenAI error: {json}");

        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

        // content is strict JSON per instructions
        var result = JsonSerializer.Deserialize<InventoryNluResult>(content ?? "{}",
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result == null)
        
            {
                R.Status = -1;
                R.Message = "GPT parse returned nothing.";
                return Created("", R);
            }

        if (result.isInventoryCheck)
        {
            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            DBTool mydbt = new DBTool(Utility.DefaultConnectionString);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("ItemCode", result.itemCode);

            DataSet DS = mydbt.RunCMD("psp_Check_Inventory");

            R.ParseDS(DS);

            return Created("", R);
        }
            R.Status = 0;
            R.Message = "Not an inventory query.";
            return Created("", R);
    }
}
