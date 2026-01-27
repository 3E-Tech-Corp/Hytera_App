using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("Api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly IDatabaseService _databaseService;

    public InventoryController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("{itemCode}")]
    public async Task<ActionResult<InventoryCheckResponse>> Get(string itemCode)
    {
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "ItemCode", itemCode }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_Check_Inventory", parameters);
        var response = ParseDataSet(dataSet);

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryCheckResponse>> Post([FromBody] InventoryCheckRequest request)
    {
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "ItemCode", request.ItemCode ?? "" }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_Check_Inventory", parameters);
        var response = ParseDataSet(dataSet);

        return Created("", response);
    }

    private static InventoryCheckResponse ParseDataSet(System.Data.DataSet dataSet)
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
