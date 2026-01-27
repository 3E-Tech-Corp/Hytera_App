using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("User/[controller]")]
[ApiController]
public class FastLoginController : ControllerBase
{
    private readonly IDatabaseService _databaseService;

    public FastLoginController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<LoginResponse>> Get(string userId)
    {
        var response = new LoginResponse();

        // Test cases for development
        switch (userId.ToLower())
        {
            case "asd":
                response.Status = 0;
                response.Message = "";
                response.UserId = "ABCDEFG";
                return Ok(response);

            case "qwe":
                response.Status = -1;
                response.Message = "Incorrect BPCODE.";
                response.UserId = "";
                return Ok(response);
        }

        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "APPID", "NewWWW" },
            { "UserID", userId }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_CheckSAP", parameters);
        response = ParseDataSet(dataSet);

        return Created("", response);
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Post([FromBody] LoginRequest request)
    {
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "APPID", request.AppId ?? "" },
            { "Email", request.Email ?? "" },
            { "PWD", request.Password ?? "" }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_CheckSAP", parameters);
        var response = ParseDataSet(dataSet);

        return Created("", response);
    }

    private static LoginResponse ParseDataSet(System.Data.DataSet dataSet)
    {
        var response = new LoginResponse();

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            response.Status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");
            response.Message = row["Message"]?.ToString();
            response.UserId = row["UserID"]?.ToString();
            response.BPCode = row["BPCode"]?.ToString();

            if (response.Status >= 0)
            {
                response.AccessToken = row["AccessToken"]?.ToString();
                response.FirstName = row["FirstName"]?.ToString();
                response.LastName = row["LastName"]?.ToString();
                response.BPName = row["BPName"]?.ToString();
                response.UserRole = row["UserRole"]?.ToString();
                response.UserRoleName = row["UserRoleName"]?.ToString();
                response.UserAccessObjects = row["UserAccessObjects"]?.ToString();
                response.BPRoleName = row["BPRoleName"]?.ToString();
                response.BPAccessObjects = row["BPAccessObjects"]?.ToString();
            }
        }
        else
        {
            response.Status = -2;
            response.Message = "Login did not return any data.";
            response.UserId = "";
        }

        return response;
    }
}
