namespace HyteraAPI.Models;

public class AppVersion
{
    public int Id { get; set; }
    public string? AppId { get; set; }
    public string? OS { get; set; }
    public string? Version { get; set; }
    public string? DownloadUrl { get; set; }
    public string? ReleaseNotes { get; set; }
    public bool IsRequired { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
