namespace HyteraAPI.Models;

public class Asset
{
    public int Id { get; set; }
    public string? ShowName { get; set; }
    public string? PhysicalPath { get; set; }
    public string? PhysicalName { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
