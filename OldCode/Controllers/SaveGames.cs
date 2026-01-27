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

    [Route("Game/[controller]")]
    [ApiController]
    public class UploadScoreController : ControllerBase
    {
        //        private string constr = @"Data Source=.\;Initial Catalog=FunTime;User ID=sa;Password=arthur1028;";
        DBTool mydbt = new DBTool(Utility.DefaultConnectionString);
       

        [HttpPost]
        public IActionResult Post([FromBody] UploadScoreRequest myApp)
        {
            UploadScoreResponse R = new UploadScoreResponse();

            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);

            mydbt.AddParam("APPID", myApp.AppID);

            mydbt.AddParam("FunTimeID", myApp.FunTimeID);

            mydbt.AddParam("Score12", myApp.Score12);
            mydbt.AddParam("Score34", myApp.Score34);
            mydbt.AddParam("StartTime", myApp.StartTime);
            mydbt.AddParam("EndTime", myApp.EndTime);
            mydbt.AddParam("GPSLng", myApp.GPSLng);
            mydbt.AddParam("GPSLat", myApp.GPSLat);
            mydbt.AddParam("GameSequence", myApp.GameSequence);

            mydbt.AddParam("EventID", myApp.EventID);
            mydbt.AddParam("MatchID", myApp.MatchID);
            mydbt.AddParam("GameNumber", myApp.GameNumber);

            mydbt.AddParam("FunTimeID1", myApp.FunTimeID1);
            mydbt.AddParam("FunTimeID2", myApp.FunTimeID2);
            mydbt.AddParam("FunTimeID3", myApp.FunTimeID3);
            mydbt.AddParam("FunTimeID4", myApp.FunTimeID4);

            DataSet DS = mydbt.RunCMD("psp_Upload_Score");


            if (DS.Tables[0].Rows.Count > 0)
            {
                R.Status = int.Parse(DS.Tables[0].Rows[0]["ReturnCode"].ToString());
                R.Message = DS.Tables[0].Rows[0]["Message"].ToString();
                R.EventID = int.Parse(DS.Tables[0].Rows[0]["EventID"].ToString());
                R.MatchID = int.Parse(DS.Tables[0].Rows[0]["MatchID"].ToString());
                R.GameNumber = int.Parse(DS.Tables[0].Rows[0]["GameNumber"].ToString());
            }
            else
            {
                R.Status = -2;
                R.Message = "Match and Game not found.";
                R.MatchID = 0;
            }
            return Created("", R);
        }

    }


    [Route("Game/[controller]")]
    [ApiController]
    public class CheckScoreController : ControllerBase
    {
        //        private string constr = @"Data Source=.\;Initial Catalog=FunTime;User ID=sa;Password=arthur1028;";
        DBTool mydbt = new DBTool(Utility.DefaultConnectionString);
         

        [HttpPost]
        public IActionResult Post([FromBody] CheckScoreRequest myApp)
        {
            CheckScoreResponse R = new CheckScoreResponse();

            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("AppID", myApp.AppID);
            mydbt.AddParam("EventID", myApp.EventID);
            mydbt.AddParam("MatchID", myApp.MatchID);
            mydbt.AddParam("GameNumber", myApp.GameNumber);

            DataSet DS = mydbt.RunCMD("psp_Check_Game_Score");

            if (DS.Tables[0].Rows.Count > 0)
            {
                R.Status = int.Parse(DS.Tables[0].Rows[0]["ReturnCode"].ToString());
                R.Message = DS.Tables[0].Rows[0]["Message"].ToString();
                R.FunTimeID1 = DS.Tables[0].Rows[0]["FunTimeID1"].ToString();
                R.FunTimeID2 = DS.Tables[0].Rows[0]["FunTimeID2"].ToString();
                R.FunTimeID3 = DS.Tables[0].Rows[0]["FunTimeID3"].ToString();
                R.FunTimeID4 = DS.Tables[0].Rows[0]["FunTimeID4"].ToString();
                R.Score12 = DS.Tables[0].Rows[0]["Score12"].ToString();
                R.Score34 = DS.Tables[0].Rows[0]["Score34"].ToString();
                R.StartTime = DateTime.Parse(DS.Tables[0].Rows[0]["StartTime"].ToString());
                R.EndTime = DateTime.Parse(DS.Tables[0].Rows[0]["EndTime"].ToString());

            }
            else
            {
                R.Status = -2;
                R.Message = "Match and Game not found.";
                R.FunTimeID1 = "";
            }
            return Created("", R);
        }
    }

    public class UploadScoreRequest
    {
        public string AppID { get; set; }
        public int EventID { get; set; }
        public int MatchID { get; set; }
        public int GameNumber { get; set; }
        public string FunTimeID { get; set; }
        public string FunTimeID1 { get; set; }
        public string FunTimeID2 { get; set; }
        public string FunTimeID3 { get; set; }
        public string FunTimeID4 { get; set; }
        public string Score12 { get; set; }
        public string Score34 { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string GPSLat { get; set; }
        public string GPSLng { get; set; }
        public string GameSequence { get; set; }
    }

    public class UploadScoreResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public int MatchID { get; set; }
        public int EventID { get; set; }
        public int GameNumber { get; set; }
    }

    public class CheckScoreRequest
    {
        public string AppID { get; set; }
        public int EventID { get; set; }
        public int MatchID { get; set; }
        public int GameNumber { get; set; }
    }
    public class CheckScoreResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string FunTimeID1 { get; set; }
        public string FunTimeID2 { get; set; }
        public string FunTimeID3 { get; set; }
        public string FunTimeID4 { get; set; }
        public string Score12 { get; set; }
        public string Score34 { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}


