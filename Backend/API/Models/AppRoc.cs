namespace HyteraAPI.Models;

public class AppRoc
{
    public int Id { get; set; }
    public string? AppId { get; set; }
    public string? FunTimeId { get; set; }
    public string? RocId { get; set; }
    public string? Roc1 { get; set; }
    public string? Roc2 { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
