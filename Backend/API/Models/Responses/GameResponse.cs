namespace HyteraAPI.Models.Responses;

public class UploadScoreResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public int EventId { get; set; }
    public int MatchId { get; set; }
    public int GameNumber { get; set; }
}

public class CheckScoreResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public string? FunTimeId1 { get; set; }
    public string? FunTimeId2 { get; set; }
    public string? FunTimeId3 { get; set; }
    public string? FunTimeId4 { get; set; }
    public string? Score12 { get; set; }
    public string? Score34 { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
