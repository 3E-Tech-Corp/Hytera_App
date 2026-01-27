namespace HyteraAPI.Models;

public class GptPrompt
{
    public int Id { get; set; }
    public string? QueryType { get; set; }
    public string? SystemText { get; set; }
    public string? UserText { get; set; }
    public string? GptModel { get; set; }
    public decimal Temperature { get; set; }
    public bool IsActive { get; set; } = true;
}
