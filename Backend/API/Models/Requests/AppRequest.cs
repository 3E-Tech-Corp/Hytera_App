namespace HyteraAPI.Models.Requests;

public class CheckVersionRequest
{
    public string? AppId { get; set; }
    public string? OS { get; set; }
    public string? Version { get; set; }
}

public class LinkRocRequest
{
    public string? AppId { get; set; }
    public string? FunTimeId { get; set; }
    public string? RocId { get; set; }
}

public class LanguageRequest
{
    public string? OS { get; set; }
    public string? LanguageCode { get; set; }
}

public class VoiceSetRequest
{
    public string? FunTimeId { get; set; }
}
