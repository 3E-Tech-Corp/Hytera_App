using HyteraAPI.Models.Requests;
using HyteraAPI.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("App/[controller]")]
[ApiController]
[Authorize]
public class LanguageController : ControllerBase
{
    [HttpGet("{languageCode}")]
    public ActionResult<LanguageResponse> Get(string languageCode)
    {
        var response = new LanguageResponse();
        var languages = new List<LanguageInfo>();

        switch (languageCode.ToLower())
        {
            case "all":
                response.Status = 1;
                response.Message = "";
                languages.Add(new LanguageInfo
                {
                    LanguageCode = "EN-US",
                    LanguageName = "American English",
                    Version = 1,
                    IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                languages.Add(new LanguageInfo
                {
                    LanguageCode = "CN",
                    LanguageName = "中文",
                    Version = 2,
                    IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=CN"
                });
                response.Languages = languages;
                break;

            case "en-us":
                response.Status = 1;
                response.Message = "";
                languages.Add(new LanguageInfo
                {
                    LanguageCode = "EN-US",
                    LanguageName = "American English",
                    Version = 1,
                    IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                response.Languages = languages;
                break;

            case "cn":
                response.Status = 1;
                response.Message = "";
                languages.Add(new LanguageInfo
                {
                    LanguageCode = "CN",
                    LanguageName = "中文",
                    Version = 2,
                    IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=CN"
                });
                response.Languages = languages;
                break;

            default:
                response.Status = -1;
                response.Message = "Invalid Language Code.";
                break;
        }

        return Ok(response);
    }

    [HttpPost]
    public ActionResult<LanguageResponse> Post([FromBody] LanguageRequest request)
    {
        var response = new LanguageResponse();
        var languages = new List<LanguageInfo>();
        var languageCode = request.LanguageCode?.ToLower() ?? "";

        switch (languageCode)
        {
            case "all":
                response.Status = 1;
                response.Message = "";
                languages.Add(new LanguageInfo
                {
                    LanguageCode = "EN-US",
                    LanguageName = "American English",
                    Version = 1,
                    IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                languages.Add(new LanguageInfo
                {
                    LanguageCode = "CN",
                    LanguageName = "中文",
                    Version = 2,
                    IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=CN"
                });
                response.Languages = languages;
                break;

            case "en-us":
                response.Status = 1;
                response.Message = "";
                languages.Add(new LanguageInfo
                {
                    LanguageCode = "EN-US",
                    LanguageName = "American English",
                    Version = 1,
                    IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                });
                response.Languages = languages;
                break;

            case "cn":
                response.Status = 1;
                response.Message = "";
                languages.Add(new LanguageInfo
                {
                    LanguageCode = "CN",
                    LanguageName = "中文",
                    Version = 2,
                    IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=CN"
                });
                response.Languages = languages;
                break;

            default:
                response.Status = -1;
                response.Message = "Invalid Language Code.";
                break;
        }

        return Created("", response);
    }
}
