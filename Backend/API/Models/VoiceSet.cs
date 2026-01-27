namespace HyteraAPI.Models;

public class VoiceSet
{
    public int Id { get; set; }
    public string? VoiceSetCode { get; set; }
    public string? VoiceSetName { get; set; }
    public string? Description { get; set; }
    public string? Version { get; set; }
    public string? AudioFileUrl { get; set; }
    public string? IndexFileUrl { get; set; }
    public string? LanguageCode { get; set; }
    public string? LanguageIndexUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
