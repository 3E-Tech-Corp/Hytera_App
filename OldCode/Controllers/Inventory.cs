using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using YYTools;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using static FunTimePIE.Controllers.CheckNewVersionController;
using System.Data.SqlTypes;

namespace FunTimePIE.Controllers
{

    [Route("Api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {

        DBTool mydbt = new DBTool(Utility.DefaultConnectionString);
         

        [HttpGet("{myID}")]
        public ActionResult<InventoryCheckResponse> Get(string myID)
        {
            InventoryCheckResponse R = new InventoryCheckResponse();
            List<ItemInfo> LS = new List<ItemInfo>();

            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("ItemCode", myID); 

            DataSet DS = mydbt.RunCMD("psp_Check_Inventory");

            R.ParseDS(DS);

            return Ok(R);

        }


        [HttpPost]
        public ActionResult<InventoryCheckResponse> Post([FromBody] InventoryCheckRequest vsr)
        {
            InventoryCheckResponse R = new InventoryCheckResponse();
            //List<ItemInfo> LS = new List<ItemInfo>();

            string RemoteIP = Utility.GetClientIpAddress(HttpContext);

            mydbt.AddParam("RemoteIP", RemoteIP);
            mydbt.AddParam("ItemCode", vsr.ItemCode );

            DataSet DS = mydbt.RunCMD("psp_Check_Inventory");

            R.ParseDS(DS);
             
            return Created("", R);
        }

        public class InventoryCheckRequest
        {
            public string ItemCode { get; set; }
        }

        public class InventoryCheckResponse
        {
            public int Status { get; set; }
            public string Message { get; set; }

            public List<ItemInfo> ItemsFound { get; set; }

            public void ParseDS(DataSet DS)
            { 
                if (DS.Tables[0].Rows.Count > 0)
                {
                    Status = int.Parse(DS.Tables[0].Rows[0]["ReturnCode"].ToString());
                    Message = DS.Tables[0].Rows[0]["Message"].ToString();
                    if (Status >= 0)
                    {
                        ItemsFound = new List<ItemInfo>();
                        foreach (DataRow row in DS.Tables[1].Rows)
                        {
                            ItemInfo item = new ItemInfo
                            {
                                ItemID = int.Parse(row["ItemID"].ToString()),
                                ItemCode = row["ItemCode"].ToString(),
                                ItemName = row["ItemName"].ToString(),
                                ItemType = row["ItemType"].ToString(),
                                Instock = int.Parse(row["Instock"].ToString()),
                                OnOrder = int.Parse(row["OnOrder"].ToString()),
                                MSRP = decimal.Parse(row["MSRP"].ToString()),
                                DealerPrice = decimal.Parse(row["DealerPrice"].ToString()),
                                ItemImageUrl = row["ItemImageUrl"].ToString(),
                                ItemLinkUrl = row["ItemLinkUrl"].ToString()
                            };
                            ItemsFound.Add(item);
                        }
                    }
                }
                else
                {
                    Status = -2;
                    Message = "No data found.";
                } 
            }

        }
        public class ItemInfo
        {
            public int ItemID { get; set; } //10 
            public string ItemCode { get; set; } //10 
            public string ItemName { get; set; }
            public string ItemType { get; set; }
            public int Instock{ get; set; }  //50
            public int OnOrder{ get; set; }
            public decimal MSRP { get; set; }
            public decimal DealerPrice { get; set; }
            public string ItemImageUrl { get; set; }
            public string ItemLinkUrl { get; set; }

        }
    }


}
