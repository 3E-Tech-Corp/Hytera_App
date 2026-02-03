using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("Game/[controller]")]
[ApiController]
[Authorize]
public class CheckScoreController : ControllerBase
{
    private readonly IDatabaseService _databaseService;

    public CheckScoreController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpPost]
    public async Task<ActionResult<CheckScoreResponse>> Post([FromBody] CheckScoreRequest request)
    {
        var response = new CheckScoreResponse();
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "AppID", request.AppId ?? "" },
            { "EventID", request.EventId },
            { "MatchID", request.MatchId },
            { "GameNumber", request.GameNumber }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_Check_Game_Score", parameters);

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            response.Status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");
            response.Message = row["Message"]?.ToString();
            response.FunTimeId1 = row["FunTimeID1"]?.ToString();
            response.FunTimeId2 = row["FunTimeID2"]?.ToString();
            response.FunTimeId3 = row["FunTimeID3"]?.ToString();
            response.FunTimeId4 = row["FunTimeID4"]?.ToString();
            response.Score12 = row["Score12"]?.ToString();
            response.Score34 = row["Score34"]?.ToString();

            if (DateTime.TryParse(row["StartTime"]?.ToString(), out var startTime))
                response.StartTime = startTime;
            if (DateTime.TryParse(row["EndTime"]?.ToString(), out var endTime))
                response.EndTime = endTime;
        }
        else
        {
            response.Status = -2;
            response.Message = "Match and Game not found.";
            response.FunTimeId1 = "";
        }

        return Created("", response);
    }
}
