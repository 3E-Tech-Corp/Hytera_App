namespace HyteraAPI.Models.Requests;

public class UploadScoreRequest
{
    public string? AppId { get; set; }
    public int EventId { get; set; }
    public int MatchId { get; set; }
    public int GameNumber { get; set; }
    public string? FunTimeId { get; set; }
    public string? FunTimeId1 { get; set; }
    public string? FunTimeId2 { get; set; }
    public string? FunTimeId3 { get; set; }
    public string? FunTimeId4 { get; set; }
    public string? Score12 { get; set; }
    public string? Score34 { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? GPSLat { get; set; }
    public string? GPSLng { get; set; }
    public string? GameSequence { get; set; }
}

public class CheckScoreRequest
{
    public string? AppId { get; set; }
    public int EventId { get; set; }
    public int MatchId { get; set; }
    public int GameNumber { get; set; }
}
