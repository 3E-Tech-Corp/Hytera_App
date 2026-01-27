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
    public class CheckNewVersionController : ControllerBase
    {
        //        private string constr = @"Data Source=.\;Initial Catalog=FunTime;User ID=sa;Password=arthur1028;";
        DBTool mydbt = new DBTool(Utility.DefaultConnectionString);

        [HttpGet("{OS}")]
        public ActionResult<CheckNewVersionResponse> Get(string OS)
        {
            CheckNewVersionResponse R = new CheckNewVersionResponse();
            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("AppID", "Test Get");
            mydbt.AddParam("OS", OS);

            DataSet DS = mydbt.RunCMD("psp_Check_New_Version");

            if (DS.Tables[0].Rows.Count > 0)
            {
                R.Status = int.Parse(DS.Tables[0].Rows[0]["ReturnCode"].ToString());
                R.Message = DS.Tables[0].Rows[0]["Message"].ToString();
            }
            else
            {
                R.Status = -1;
                R.Message = "Invalid OS.";
            }

            return Created("", R);

        }

        [HttpPost]
        public IActionResult Post([FromBody] CheckNewVersionRequest myApp)
        {
            CheckNewVersionResponse R = new CheckNewVersionResponse();

            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("AppID", myApp.AppID);
            mydbt.AddParam("OS", myApp.OS);
            mydbt.AddParam("Ver", myApp.Version);

            DataSet DS = mydbt.RunCMD("psp_Check_New_Version");

            if (DS.Tables[0].Rows.Count > 0)
            {
                R.Status = int.Parse(DS.Tables[0].Rows[0]["ReturnCode"].ToString());
                R.Message = DS.Tables[0].Rows[0]["Message"].ToString();
            }
            else
            {
                R.Status = -1;
                R.Message = "Invalid OS.";
            }

            return Created("", R);
        }


        public class CheckNewVersionRequest
        {
            public string AppID { get; set; }
            public string OS { get; set; }
            public string Version { get; set; }
        }

        public class CheckNewVersionResponse
        {
            public int Status { get; set; }
            public string Message { get; set; }
        }


    }

    [Route("App/[controller]")]
    [ApiController]
    public class LinkNewROCController : ControllerBase
    {

        //        private string constr = @"Data Source=.\;Initial Catalog=FunTime;User ID=sa;Password=arthur1028;";
        DBTool mydbt = new DBTool(Utility.DefaultConnectionString);

        [HttpGet("{APPID}")]
        public ActionResult<CheckNewVersionResponse> Get(string APPID)
        {
            LinkNewROCResponse R = new LinkNewROCResponse();
            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("AppID", APPID); 
             

            DataSet DS = mydbt.RunCMD("psp_Get_APP_ROCs");

            if (DS.Tables[0].Rows.Count > 0)
            {
                R.Status = int.Parse(DS.Tables[0].Rows[0]["ReturnCode"].ToString());
                R.ROC1 = DS.Tables[0].Rows[0]["ROC1"].ToString();
                R.ROC2 = DS.Tables[0].Rows[0]["ROC2"].ToString();
                R.Message = DS.Tables[0].Rows[0]["Message"].ToString();
            }
            else
            {
                R.Status = -1;
                R.Message = "Invalid APP ROC Get Request.";
            }

            return Created("", R);

        }

        [HttpPost]
        public IActionResult Post([FromBody] LinkNewROCRequest myApp)
        {
            LinkNewROCResponse R = new LinkNewROCResponse();

            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("AppID", myApp.AppID);
            mydbt.AddParam("FTID", myApp.FTID);
            mydbt.AddParam("ROCID", myApp.ROCID);

            DataSet DS = mydbt.RunCMD("psp_Link_New_ROC");

            if (DS.Tables[0].Rows.Count > 0)
            {
                R.Status = int.Parse(DS.Tables[0].Rows[0]["ReturnCode"].ToString());
                R.Message = DS.Tables[0].Rows[0]["Message"].ToString();
            }
            else
            {
                R.Status = -1;
                R.Message = "Invalid Link ROC Request Data.";
            }

            return Created("", R);
        }


        public class LinkNewROCRequest
        {
            public string AppID { get; set; }
            public string FTID { get; set; }
            public string ROCID { get; set; }
        }

        public class LinkNewROCResponse
        {
            public int Status { get; set; }
            public string Message { get; set; }
            public string ROC1 { get; set; }
            public string ROC2 { get; set; }
        }

    }

}
