using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("User/[controller]")]
[ApiController]
[AllowAnonymous]
public class ResetPasswordController : ControllerBase
{
    private readonly IDatabaseService _databaseService;

    public ResetPasswordController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("{email}")]
    public async Task<ActionResult<UpdatePasswordResponse>> Get(string email)
    {
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "userID", "0" },
            { "Token", email },
            { "PWD", "" }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_UpdateSAP_PWD", parameters);
        var response = ParseDataSet(dataSet);

        return Created("", response);
    }

    [HttpPost]
    public async Task<ActionResult<UpdatePasswordResponse>> Post([FromBody] UpdatePasswordRequest request)
    {
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "userID", request.UserId ?? "" },
            { "Token", request.Token ?? "" },
            { "PWD", request.Password ?? "" }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_UpdateSAP_PWD", parameters);
        var response = ParseDataSet(dataSet);

        return Created("", response);
    }

    private static UpdatePasswordResponse ParseDataSet(System.Data.DataSet dataSet)
    {
        var response = new UpdatePasswordResponse();

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            response.Status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");
            response.Message = row["Message"]?.ToString();
            response.UserId = row["UserID"]?.ToString();
            response.Token = row["Token"]?.ToString();
        }
        else
        {
            response.Status = -2;
            response.Message = "Password reset did not return any data.";
            response.UserId = "";
        }

        return response;
    }
}
