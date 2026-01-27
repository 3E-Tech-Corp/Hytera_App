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

    [Route("User/[controller]")]
    [ApiController]
    public class VoicesetController : ControllerBase
    {
        [HttpGet("{myID}")]
        public ActionResult<VoiceSetResponse> Get(string myID)
        {
            VoiceSetResponse R = new VoiceSetResponse();
            List<VoiceSetInfo> LS = new List<VoiceSetInfo>();

            switch (myID.ToLower())
            {
                case "asd":

                    R.Status = 0;
                    R.Message = "";
                    LS.Add(new VoiceSetInfo()
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
                    LS.Add(new VoiceSetInfo()
                    {
                        VoiceSetName = "Eva",
                        VoiceSetCode = "EN-US-Eva",
                        Description = "American Female Voice by Adam.",
                        Version = "2.0.0",
                        IndexFileUrl = "http://www.funtimepb.com/Audio/GetIndex.aspx?VS=EN_Eva",
                        AudioFileUrl = "http://www.funtimepb.com/Audio/GetVoiceset.aspx?VS=EN_Eva",
                        LanguageCode = "EN-US",
                        LanguageIndexUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                    });
                    R.VoiceSets = LS;
                    break;
                case "qwe":

                    R.Status = 0;
                    R.Message = "";
                    LS.Add(new VoiceSetInfo()
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
                    R.VoiceSets = LS;
                    break;

                default:
                    R.Status = -1;
                    R.Message = "User not allowed to download any voice set.";
                    break;
            }

            return Ok(R);

        }


        [HttpPost]
        public ActionResult<VoiceSetResponse> Post([FromBody] VoiceSetRequest vsr)
        {
            VoiceSetResponse R = new VoiceSetResponse();
            List<VoiceSetInfo> LS = new List<VoiceSetInfo>();

            switch (vsr.FunTimeID.ToLower())
            {
                case "asd":

                    R.Status = 0;
                    R.Message = "";
                    LS.Add(new VoiceSetInfo()
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
                    LS.Add(new VoiceSetInfo()
                    {
                        VoiceSetName = "Eva",
                        VoiceSetCode = "EN-US-Eva",
                        Description = "American Female Voice by Adam.",
                        Version = "2.0.0",
                        IndexFileUrl = "http://www.funtimepb.com/Audio/GetIndex.aspx?VS=EN_Eva",
                        AudioFileUrl = "http://www.funtimepb.com/Audio/GetVoiceset.aspx?VS=EN_Eva",
                        LanguageCode = "EN-US",
                        LanguageIndexUrl = "http://www.funtimepb.com/Languages/GetIndex.aspx?Lang=EN-US"
                    });
                    R.VoiceSets = LS;
                    break;
                case "qwe":

                    R.Status = 0;
                    R.Message = "";
                    LS.Add(new VoiceSetInfo()
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
                    R.VoiceSets = LS;
                    break;

                default:
                    R.Status = -1;
                    R.Message = "User not allowed to download any voice set.";
                    break;
            }
            return Created("", R);
        }

        public class VoiceSetRequest
        {
            public string FunTimeID { get; set; }
        }

        public class VoiceSetResponse
        {
            public int Status { get; set; }
            public string Message { get; set; }

            public List<VoiceSetInfo> VoiceSets { get; set; }

        }
        public class VoiceSetInfo
        {
            public string VoiceSetCode { get; set; } //10 
            public string Version { get; set; }
            public string VoiceSetName { get; set; }  //50
            public string Description { get; set; }
            public string AudioFileUrl { get; set; }
            public string IndexFileUrl { get; set; }
            public string LanguageCode { get; set; }
            public string LanguageIndexUrl { get; set; }
        }
    }


}
