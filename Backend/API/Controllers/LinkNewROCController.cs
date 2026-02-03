using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("App/[controller]")]
[ApiController]
[Authorize]
public class LinkNewROCController : ControllerBase
{
    private readonly IDatabaseService _databaseService;

    public LinkNewROCController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("{appId}")]
    public async Task<ActionResult<LinkRocResponse>> Get(string appId)
    {
        var response = new LinkRocResponse();
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "AppID", appId }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_Get_APP_ROCs", parameters);

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            response.Status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");
            response.Roc1 = row["ROC1"]?.ToString();
            response.Roc2 = row["ROC2"]?.ToString();
            response.Message = row["Message"]?.ToString();
        }
        else
        {
            response.Status = -1;
            response.Message = "Invalid APP ROC Get Request.";
        }

        return Created("", response);
    }

    [HttpPost]
    public async Task<ActionResult<LinkRocResponse>> Post([FromBody] LinkRocRequest request)
    {
        var response = new LinkRocResponse();
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "AppID", request.AppId ?? "" },
            { "FTID", request.FunTimeId ?? "" },
            { "ROCID", request.RocId ?? "" }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_Link_New_ROC", parameters);

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            response.Status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");
            response.Message = row["Message"]?.ToString();
        }
        else
        {
            response.Status = -1;
            response.Message = "Invalid Link ROC Request Data.";
        }

        return Created("", response);
    }
}
