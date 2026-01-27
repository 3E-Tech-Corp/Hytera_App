namespace HyteraAPI.Models;

public class Language
{
    public int Id { get; set; }
    public string? LanguageCode { get; set; }
    public string? LanguageName { get; set; }
    public int Version { get; set; }
    public string? IndexFileUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
