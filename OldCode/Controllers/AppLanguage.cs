using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using YYTools;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using static FunTimePIE.Controllers.CheckNewVersionController;

namespace FunTimePIE.Controllers
{

    [Route("App/[controller]")]
    [ApiController]
    public class LanguageController : ControllerBase
    {
        [HttpGet("{myLang}")]
        public ActionResult<LanguageResponse> Get(string myLang)
        {
            LanguageResponse R = new LanguageResponse();
            List<LanguageInfo> LS = new List<LanguageInfo>();

            switch (myLang.ToLower())
            {
                case "all":

                    R.Status = 1;
                    R.Message = "";
                    LS.Add(new LanguageInfo()
                    {
                        LanguageCode = "EN-US",
                        LanguageName = "American English",
                        Version = 1,
                        IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                    });
                    LS.Add(new LanguageInfo()
                    {
                        LanguageCode = "CN",
                        LanguageName = "中文",
                        Version = 2,
                        IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=CN"
                    });
                    R.Languages = LS;
                    break;
                case "en-us":

                    R.Status = 1;
                    R.Message = "";
                    LS.Add(new LanguageInfo()
                    {
                        LanguageCode = "EN-US",
                        LanguageName = "American English",
                        Version = 1,
                        IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                    });
                    R.Languages = LS;
                    break;
                case "cn":

                    R.Status = 1;
                    R.Message = "";
                    LS.Add(new LanguageInfo()
                    {
                        LanguageCode = "CN",
                        LanguageName = "中文",
                        Version = 2,
                        IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=CN"
                    });
                    R.Languages = LS;
                    break;
                default:
                    R.Status = -1;
                    R.Message = "Invalid Language Code.";
                    break;
            }
            //return CreatedAtAction("LanguageResponse", R);
            return Ok (R);

        }


        [HttpPost]
        public ActionResult<LanguageResponse> Post([FromBody] LanguageRequest myLang)
        {
            LanguageResponse R = new LanguageResponse();
            List<LanguageInfo> LS = new List<LanguageInfo>();

            switch (myLang.LanguageCode.ToLower())
            {
                case "all":
                     
                        R.Status = 1;
                        R.Message = "";
                    LS.Add(new LanguageInfo()
                    {
                        LanguageCode = "EN-US",
                        LanguageName = "American English",
                        Version = 1,
                        IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                    }) ;
                    LS.Add(new LanguageInfo()
                    {
                        LanguageCode = "CN",
                        LanguageName = "中文",
                        Version = 2,
                        IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=CN"
                    });
                    R.Languages = LS;
                    break;
                case "en-us":
                    
                        R.Status = 1;
                        R.Message = "";
                    LS.Add(new LanguageInfo()
                    {
                        LanguageCode = "EN-US",
                        LanguageName = "American English",
                        Version = 1,
                        IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                    });
                    R.Languages = LS;
                    break;
                case "cn":

                    R.Status = 1;
                    R.Message = "";
                    LS.Add(new LanguageInfo()
                    {
                        LanguageCode = "CN",
                        LanguageName = "中文",
                        Version = 2,
                        IndexFileUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=CN"
                    });
                    R.Languages = LS;
                    break;
                default:
                    R.Status = -1;
                    R.Message = "Invalid Language Code.";
                    break;
            }
            return Created("", R);
        }

        public class LanguageRequest
        {
            public string OS { get; set; }
            public string LanguageCode { get; set; }
        }

        public class LanguageResponse
        {
            public int Status { get; set; }
            public string Message { get; set; }

            public List <LanguageInfo> Languages { get; set; }

        }
        public class LanguageInfo
        {
            public int Version { get; set; }
            public string LanguageCode { get; set; }
            public string LanguageName { get; set; }
            public string IndexFileUrl{ get; set; }
        }
    }


}
