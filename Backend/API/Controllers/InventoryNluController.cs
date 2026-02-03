using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace HyteraAPI.Controllers;

[Route("api/nlu")]
[ApiController]
[Authorize]
public class InventoryNluController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IDatabaseService _databaseService;

    public InventoryNluController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IDatabaseService databaseService)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _databaseService = databaseService;
    }

    [HttpPost("inventory")]
    public async Task<ActionResult<InventoryCheckResponse>> Inventory([FromBody] NluRequest request)
    {
        var response = new InventoryCheckResponse();
        var apiKey = _configuration["OpenAI:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            response.Status = -1;
            response.Message = "Missing OpenAI API key.";
            return Created("", response);
        }

        // Get prompts from database
        var promptParams = new Dictionary<string, object>
        {
            { "Query", "Inventory" },
            { "Request", request.Text ?? "" }
        };

        var promptDataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_GPT_Prompts", promptParams);

        if (promptDataSet.Tables.Count == 0 || promptDataSet.Tables[0].Rows.Count == 0)
        {
            response.Status = -1;
            response.Message = "Failed to get GPT prompts.";
            return Created("", response);
        }

        var promptRow = promptDataSet.Tables[0].Rows[0];
        var systemText = promptRow["system_text"]?.ToString() ?? "";
        var userText = promptRow["user_text"]?.ToString() ?? "";
        var gptModel = promptRow["GPTmodel"]?.ToString() ?? "gpt-4";
        var temperature = int.Parse(promptRow["temperature"]?.ToString() ?? "0");

        // Call OpenAI API
        var payload = new
        {
            model = gptModel,
            messages = new object[]
            {
                new { role = "system", content = systemText },
                new { role = "user", content = userText }
            },
            temperature = temperature
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        httpRequest.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var httpResponse = await _httpClient.SendAsync(httpRequest);
        var json = await httpResponse.Content.ReadAsStringAsync();

        if (!httpResponse.IsSuccessStatusCode)
        {
            return Problem($"OpenAI error: {json}");
        }

        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        var nluResult = JsonSerializer.Deserialize<NluResult>(content ?? "{}",
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (nluResult == null)
        {
            response.Status = -1;
            response.Message = "GPT parse returned nothing.";
            return Created("", response);
        }

        if (nluResult.IsInventoryCheck)
        {
            var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

            var inventoryParams = new Dictionary<string, object>
            {
                { "RemoteIP", remoteIp },
                { "ItemCode", nluResult.ItemCode ?? "" }
            };

            var inventoryDataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_Check_Inventory", inventoryParams);
            response = ParseInventoryDataSet(inventoryDataSet);

            return Created("", response);
        }

        response.Status = 0;
        response.Message = "Not an inventory query.";
        return Created("", response);
    }

    private static InventoryCheckResponse ParseInventoryDataSet(System.Data.DataSet dataSet)
    {
        var response = new InventoryCheckResponse();

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            response.Status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");
            response.Message = row["Message"]?.ToString();

            if (response.Status >= 0 && dataSet.Tables.Count > 1)
            {
                response.ItemsFound = new List<ItemInfo>();
                foreach (System.Data.DataRow itemRow in dataSet.Tables[1].Rows)
                {
                    response.ItemsFound.Add(new ItemInfo
                    {
                        ItemId = int.Parse(itemRow["ItemID"]?.ToString() ?? "0"),
                        ItemCode = itemRow["ItemCode"]?.ToString(),
                        ItemName = itemRow["ItemName"]?.ToString(),
                        ItemType = itemRow["ItemType"]?.ToString(),
                        Instock = int.Parse(itemRow["Instock"]?.ToString() ?? "0"),
                        OnOrder = int.Parse(itemRow["OnOrder"]?.ToString() ?? "0"),
                        MSRP = decimal.Parse(itemRow["MSRP"]?.ToString() ?? "0"),
                        DealerPrice = decimal.Parse(itemRow["DealerPrice"]?.ToString() ?? "0"),
                        ItemImageUrl = itemRow["ItemImageUrl"]?.ToString(),
                        ItemLinkUrl = itemRow["ItemLinkUrl"]?.ToString()
                    });
                }
            }
        }
        else
        {
            response.Status = -2;
            response.Message = "No data found.";
        }

        return response;
    }
}
