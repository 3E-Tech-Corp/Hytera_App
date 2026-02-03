using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("User/[controller]")]
[ApiController]
[Authorize]
public class VoicesetController : ControllerBase
{
    [HttpGet("{userId}")]
    public ActionResult<VoiceSetResponse> Get(string userId)
    {
        var response = new VoiceSetResponse();
        var voiceSets = new List<VoiceSetInfo>();

        switch (userId.ToLower())
        {
            case "asd":
                response.Status = 0;
                response.Message = "";
                voiceSets.Add(new VoiceSetInfo
                {
                    VoiceSetName = "Adam",
                    VoiceSetCode = "EN-US-Adam",
                    Description = "American Male Voice by Adam.",
                    Version = "1.0.0",
                    IndexFileUrl = "http://www.funtimepb.com/Audio/GetIndex.aspx?VS=EN_Adam",
                    AudioFileUrl = "http://www.funtimepb.com/Audio/GetVoiceset.aspx?VS=EN_Adam",
                    LanguageCode = "EN-US",
                    LanguageIndexUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                voiceSets.Add(new VoiceSetInfo
                {
                    VoiceSetName = "Eva",
                    VoiceSetCode = "EN-US-Eva",
                    Description = "American Female Voice by Eva.",
                    Version = "2.0.0",
                    IndexFileUrl = "http://www.funtimepb.com/Audio/GetIndex.aspx?VS=EN_Eva",
                    AudioFileUrl = "http://www.funtimepb.com/Audio/GetVoiceset.aspx?VS=EN_Eva",
                    LanguageCode = "EN-US",
                    LanguageIndexUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                response.VoiceSets = voiceSets;
                break;

            case "qwe":
                response.Status = 0;
                response.Message = "";
                voiceSets.Add(new VoiceSetInfo
                {
                    VoiceSetName = "Adam",
                    VoiceSetCode = "EN-US-Adam",
                    Description = "American Male Voice by Adam.",
                    Version = "1.0.0",
                    IndexFileUrl = "http://www.funtimepb.com/Audio/GetIndex.aspx?VS=EN_Adam",
                    AudioFileUrl = "http://www.funtimepb.com/Audio/GetVoiceset.aspx?VS=EN_Adam",
                    LanguageCode = "EN-US",
                    LanguageIndexUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                response.VoiceSets = voiceSets;
                break;

            default:
                response.Status = -1;
                response.Message = "User not allowed to download any voice set.";
                break;
        }

        return Ok(response);
    }

    [HttpPost]
    public ActionResult<VoiceSetResponse> Post([FromBody] VoiceSetRequest request)
    {
        var response = new VoiceSetResponse();
        var voiceSets = new List<VoiceSetInfo>();
        var funTimeId = request.FunTimeId?.ToLower() ?? "";

        switch (funTimeId)
        {
            case "asd":
                response.Status = 0;
                response.Message = "";
                voiceSets.Add(new VoiceSetInfo
                {
                    VoiceSetName = "Adam",
                    VoiceSetCode = "EN-US-Adam",
                    Description = "American Male Voice by Adam.",
                    Version = "1.0.0",
                    IndexFileUrl = "http://www.funtimepb.com/Audio/GetIndex.aspx?VS=EN_Adam",
                    AudioFileUrl = "http://www.funtimepb.com/Audio/GetVoiceset.aspx?VS=EN_Adam",
                    LanguageCode = "EN-US",
                    LanguageIndexUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                voiceSets.Add(new VoiceSetInfo
                {
                    VoiceSetName = "Eva",
                    VoiceSetCode = "EN-US-Eva",
                    Description = "American Female Voice by Eva.",
                    Version = "2.0.0",
                    IndexFileUrl = "http://www.funtimepb.com/Audio/GetIndex.aspx?VS=EN_Eva",
                    AudioFileUrl = "http://www.funtimepb.com/Audio/GetVoiceset.aspx?VS=EN_Eva",
                    LanguageCode = "EN-US",
                    LanguageIndexUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                response.VoiceSets = voiceSets;
                break;

            case "qwe":
                response.Status = 0;
                response.Message = "";
                voiceSets.Add(new VoiceSetInfo
                {
                    VoiceSetName = "Adam",
                    VoiceSetCode = "EN-US-Adam",
                    Description = "American Male Voice by Adam.",
                    Version = "1.0.0",
                    IndexFileUrl = "http://www.funtimepb.com/Audio/GetIndex.aspx?VS=EN_Adam",
                    AudioFileUrl = "http://www.funtimepb.com/Audio/GetVoiceset.aspx?VS=EN_Adam",
                    LanguageCode = "EN-US",
                    LanguageIndexUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                response.VoiceSets = voiceSets;
                break;

            default:
                response.Status = -1;
                response.Message = "User not allowed to download any voice set.";
                break;
        }

        return Created("", response);
    }
}
