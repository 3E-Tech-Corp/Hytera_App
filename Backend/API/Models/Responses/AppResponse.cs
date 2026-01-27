namespace HyteraAPI.Models.Responses;

public class CheckVersionResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
}

public class LinkRocResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public string? Roc1 { get; set; }
    public string? Roc2 { get; set; }
}

public class LanguageResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public List<LanguageInfo>? Languages { get; set; }
}

public class LanguageInfo
{
    public int Version { get; set; }
    public string? LanguageCode { get; set; }
    public string? LanguageName { get; set; }
    public string? IndexFileUrl { get; set; }
}

public class VoiceSetResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public List<VoiceSetInfo>? VoiceSets { get; set; }
}

public class VoiceSetInfo
{
    public string? VoiceSetCode { get; set; }
    public string? Version { get; set; }
    public string? VoiceSetName { get; set; }
    public string? Description { get; set; }
    public string? AudioFileUrl { get; set; }
    public string? IndexFileUrl { get; set; }
    public string? LanguageCode { get; set; }
    public string? LanguageIndexUrl { get; set; }
}

public class FileBase64Response
{
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
    public string? Base64Data { get; set; }
}
