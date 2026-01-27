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
    public class FastLoginController : ControllerBase
    {

        DBTool mydbt = new DBTool(Utility.DefaultConnectionString);


        private LoginResponse ParseDS(DataSet DS)
        {
            LoginResponse R = new LoginResponse();

            if (DS.Tables[0].Rows.Count > 0)
            {
                R.Status = int.Parse(DS.Tables[0].Rows[0]["ReturnCode"].ToString());
                R.Message = DS.Tables[0].Rows[0]["Message"].ToString();

                R.UserID = DS.Tables[0].Rows[0]["UserID"].ToString();

                R.BPCode = DS.Tables[0].Rows[0]["BPCode"].ToString();

                if (R.Status >= 0)
                {
                    R.AccessToken = DS.Tables[0].Rows[0]["AccessToken"].ToString();
                    R.FirstName = DS.Tables[0].Rows[0]["FirstName"].ToString();
                    R.LastName = DS.Tables[0].Rows[0]["LastName"].ToString();
                    R.BPName = DS.Tables[0].Rows[0]["BPName"].ToString();
                    R.UserRole = DS.Tables[0].Rows[0]["UserRole"].ToString();
                    R.UserRoleName = DS.Tables[0].Rows[0]["UserRoleName"].ToString();
                    R.UserAccessObjects = DS.Tables[0].Rows[0]["UserAccessObjects"].ToString();
                    R.BPRoleName = DS.Tables[0].Rows[0]["BPRoleName"].ToString(); 
                    R.BPAccessObjects = DS.Tables[0].Rows[0]["BPAccessObjects"].ToString();
                }
            }
            else
            {
                R.Status = -2;
                R.Message = "Restful Login GET did not return any data.";
                R.UserID = "";
            }

            return R;
        }

        [HttpGet("{myID}")]
        public ActionResult<LoginResponse> Get(string myID)
        {
            LoginResponse R = new LoginResponse();


            switch (myID.ToLower())
            {
                case "asd":

                    R.Status = 0;
                    R.Message = "";
                    R.UserID = "ABCDEFG";
                    break; ;
                case "qwe":

                    R.Status = -1;
                    R.Message = "Incorrect BPCODE.";
                    R.UserID = "";
                    break;

                default:
                    string RemoteIP = Utility.GetClientIpAddress(HttpContext);

                    mydbt.AddParam("RemoteIP", RemoteIP);
                    mydbt.AddParam("APPID", "NewWWW");
                    mydbt.AddParam("UserID", myID);

                    DataSet DS = mydbt.RunCMD("psp_CheckSAP");

                    R = ParseDS(DS);
                    return Created("", R);
            }
            return Ok(R);

        }


        [HttpPost]
        public ActionResult<LoginResponse> Post([FromBody] FastLoginRequest vsr)
        {
            LoginResponse R = new LoginResponse();
            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("APPID", vsr.AppID);
            mydbt.AddParam("Email", vsr.Email);
            mydbt.AddParam("PWD", vsr.Password);

            DataSet DS = mydbt.RunCMD("psp_CheckSAP");

            R = ParseDS(DS);
            return Created("", R);
        }

        public class FastLoginRequest
        {
            public string AppID { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }

    [Route("User/[controller]")]
    [ApiController]
    public class ResetPasswordController : ControllerBase
    {

        DBTool mydbt = new DBTool(Utility.DefaultConnectionString);

        private UpdatePWDResponse ParseDS(DataSet DS)
        {
            UpdatePWDResponse R = new UpdatePWDResponse();


            if (DS.Tables[0].Rows.Count > 0)
            {
                R.Status = int.Parse(DS.Tables[0].Rows[0]["ReturnCode"].ToString());
                R.Message = DS.Tables[0].Rows[0]["Message"].ToString();

                R.UserID = DS.Tables[0].Rows[0]["UserID"].ToString();

                R.Token = DS.Tables[0].Rows[0]["Token"].ToString();


            }

            else
            {
                R.Status = -2;
                R.Message = "Restful Login GET did not return any data.";
                R.UserID = "";
            }

            return R;
        }


        [HttpGet("{email}")]
        public ActionResult<UpdatePWDResponse> Get(string email)
        {
            UpdatePWDResponse R = new UpdatePWDResponse();



            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("userID", "0");
            mydbt.AddParam("Token", email);
            mydbt.AddParam("PWD", "");

            DataSet DS = mydbt.RunCMD("psp_UpdateSAP_PWD");

            R = ParseDS(DS);
            return Created("", R);

        }


        [HttpPost]
        public ActionResult<UpdatePWDResponse> Post([FromBody] UpdatePWDRequest vsr)
        {
            UpdatePWDResponse R = new UpdatePWDResponse();
            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("userID", vsr.UserID);
            mydbt.AddParam("Token", vsr.Token);
            mydbt.AddParam("PWD", vsr.Password);

            DataSet DS = mydbt.RunCMD("psp_UpdateSAP_PWD");

            R = ParseDS(DS);
            return Created("", R);
        }
    }




    public class UpdatePWDRequest
    {
        public string UserID { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }

    }

    public class UpdatePWDResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string UserID { get; set; }
        public string Token { get; set; }

    }

    public class LoginRequest
    {
        public string AppID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }


    public class LoginResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }

        public string AccessToken { get; set; }

        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserRole { get; set; }
        public string UserRoleName { get; set; }
        public string UserAccessObjects { get; set; }

        public string BPCode { get; set; }
        public string BPName { get; set; }

        public string BPRoleName { get; set; }
        public string BPAccessObjects { get; set; }

    }


}
