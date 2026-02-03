using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("App/[controller]")]
[ApiController]
[Authorize]
public class CheckNewVersionController : ControllerBase
{
    private readonly IDatabaseService _databaseService;

    public CheckNewVersionController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("{os}")]
    public async Task<ActionResult<CheckVersionResponse>> Get(string os)
    {
        var response = new CheckVersionResponse();
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "AppID", "Test Get" },
            { "OS", os }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_Check_New_Version", parameters);

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            response.Status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");
            response.Message = row["Message"]?.ToString();
        }
        else
        {
            response.Status = -1;
            response.Message = "Invalid OS.";
        }

        return Created("", response);
    }

    [HttpPost]
    public async Task<ActionResult<CheckVersionResponse>> Post([FromBody] CheckVersionRequest request)
    {
        var response = new CheckVersionResponse();
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "AppID", request.AppId ?? "" },
            { "OS", request.OS ?? "" },
            { "Ver", request.Version ?? "" }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_Check_New_Version", parameters);

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            response.Status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");
            response.Message = row["Message"]?.ToString();
        }
        else
        {
            response.Status = -1;
            response.Message = "Invalid OS.";
        }

        return Created("", response);
    }
}
