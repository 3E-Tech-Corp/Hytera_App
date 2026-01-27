using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("Game/[controller]")]
[ApiController]
public class UploadScoreController : ControllerBase
{
    private readonly IDatabaseService _databaseService;

    public UploadScoreController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpPost]
    public async Task<ActionResult<UploadScoreResponse>> Post([FromBody] UploadScoreRequest request)
    {
        var response = new UploadScoreResponse();
        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "APPID", request.AppId ?? "" },
            { "FunTimeID", request.FunTimeId ?? "" },
            { "Score12", request.Score12 ?? "" },
            { "Score34", request.Score34 ?? "" },
            { "StartTime", request.StartTime },
            { "EndTime", request.EndTime },
            { "GPSLng", request.GPSLng ?? "" },
            { "GPSLat", request.GPSLat ?? "" },
            { "GameSequence", request.GameSequence ?? "" },
            { "EventID", request.EventId },
            { "MatchID", request.MatchId },
            { "GameNumber", request.GameNumber },
            { "FunTimeID1", request.FunTimeId1 ?? "" },
            { "FunTimeID2", request.FunTimeId2 ?? "" },
            { "FunTimeID3", request.FunTimeId3 ?? "" },
            { "FunTimeID4", request.FunTimeId4 ?? "" }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_Upload_Score", parameters);

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            response.Status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");
            response.Message = row["Message"]?.ToString();
            response.EventId = int.Parse(row["EventID"]?.ToString() ?? "0");
            response.MatchId = int.Parse(row["MatchID"]?.ToString() ?? "0");
            response.GameNumber = int.Parse(row["GameNumber"]?.ToString() ?? "0");
        }
        else
        {
            response.Status = -2;
            response.Message = "Match and Game not found.";
            response.MatchId = 0;
        }

        return Created("", response);
    }
}
