using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using YYTools;
using static FunTimePIE.Controllers.CheckNewVersionController;

namespace FunTimePIE.Controllers
{
    [Route("Asset")]
    [ApiController]
    public class AssetsManagerController : ControllerBase
    {

        private string RootPath = Utility.DefaultAssetRoot;


        private string GetFileName(int FID)
        {
            DBTool myDBT = new DBTool(Utility.DefaultConnectionString);

            myDBT.AddParam("FID", FID.ToString());
            //            myDBT.AddParam("FT", FT);
            DataSet DS = myDBT.RunCMD("psp_V2_Get_User_File_Info");
            myDBT = null;

            string ShowAs = "";
            string Phy_Path = "";
            string Phy_Name = "";
            string FullName = "";


            if (DS.Tables[0].Rows.Count > 0)
            {
                ShowAs = DS.Tables[0].Rows[0]["Show_Name"].ToString();
                Phy_Path = DS.Tables[0].Rows[0]["Phy_Path"].ToString();
                Phy_Name = DS.Tables[0].Rows[0]["Phy_Name"].ToString();
                FullName = System.IO.Path.Combine(System.IO.Path.Combine(RootPath, Phy_Path), Phy_Name);
                if (System.IO.File.Exists(FullName))
                    return FullName;
            }
            else
                return "";
            return "";
        }

        /// <summary>
        /// Shows the image file from doc vault
        /// </summary>
        /// <param name="ImageID"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        [HttpGet("Image/{ImageID:int}/{Width:int?}/{Height:int?}")]
        public IActionResult GetImage(int ImageID, int Width = 800, int Height = 600)
        {


            string NotFound = "";
            string WaterMark = "";

            NetCoreImageTools IT = new();
            IT.ImageDocRoot = RootPath;
            IT.NotFoundImageCode = NotFound;
            string FullName = GetFileName(ImageID);

            string ShowAs = System.IO.Path.GetFileName(FullName);

            if (string.IsNullOrEmpty(FullName))

            {
                switch (NotFound.ToLower())
                {
                    case "nologo":
                        ShowAs = "NoLogo.png";
                        FullName = System.IO.Path.Combine(RootPath, ShowAs);
                        break;
                    case "nophoto":
                        ShowAs = "NoPhoto.png";
                        FullName = System.IO.Path.Combine(RootPath, ShowAs);
                        break;
                    default:
                        ShowAs = "NotFound.png";
                        FullName = System.IO.Path.Combine(RootPath, ShowAs);
                        break;
                }
            }
        LoadImageFile:
            IT.LoadImageFile(FullName, ShowAs);

            if (Width <= 0 || Height <= 0)
                IT.UseOrigImage();
            else
                IT.ResizeImage(Width, Height);

            try
            {
                if (!string.IsNullOrEmpty(WaterMark))
                {
                    //  do WaterMark 
                    IT.AddWaterMark(WaterMark, "Test");
                }
            }
            catch
            { }


            switch (IT.ReturnImageType)

            {
                case "png":
                    return File(IT.ConverttoPng(), "image/png");
                case "jpg":
                default:
                    return File(IT.ConvertToJpeg(), "image/jpeg");
            }

        }



        [HttpGet("File/{FID:int}")]
        public IActionResult GetFile(int FID)
        {



            string FullName = GetFileName(FID);

            string ShowAs = System.IO.Path.GetFileName(FullName);

            if (string.IsNullOrEmpty(FullName))

            {
                return NotFound();
            }
            var fileBytes = System.IO.File.ReadAllBytes(FullName);
            var contentType = Utility.GetContentType(ShowAs);

            return File(fileBytes, contentType, ShowAs);

        }

        [HttpGet("viewFile/{FID}")]
        public IActionResult ViewFile(int FID)
        {



            string FullName = GetFileName(FID);

            string ShowAs = System.IO.Path.GetFileName(FullName);

            if (string.IsNullOrEmpty(FullName))

            {
                return NotFound();
            }
            var fileBytes = System.IO.File.ReadAllBytes(FullName);
            var contentType = Utility.GetContentType(ShowAs);


            // This will display in browser instead of downloading
            Response.Headers.Add("Content-Disposition", "inline");

            return File(fileBytes, contentType);
        }

        // 6. Return file with range support (for large files/video streaming)
        [HttpGet("streamvideo/{FID:int}")]
        public async Task<IActionResult> StreamVideo(int FID)
        {



            string filePath = GetFileName(FID);

            string ShowAs = System.IO.Path.GetFileName(filePath);

            if (string.IsNullOrEmpty(filePath))

            {
                return NotFound();
            }

            var fileInfo = new FileInfo(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            // Support range requests for video streaming
            var rangeHeader = Request.Headers["Range"].ToString();
            if (!string.IsNullOrEmpty(rangeHeader))
            {
                return await HandleRangeRequest(fileStream, rangeHeader, "video/mp4", fileInfo.Length);
            }

            return File(fileStream, "video/mp4", enableRangeProcessing: true);
        }

        // Helper for range requests (video streaming)
        private async Task<IActionResult> HandleRangeRequest(FileStream fileStream, string rangeHeader, string contentType, long fileLength)
        {
            var ranges = rangeHeader.Replace("bytes=", "").Split('-');
            var start = long.Parse(ranges[0]);
            var end = ranges.Length > 1 && !string.IsNullOrEmpty(ranges[1])
                ? long.Parse(ranges[1])
                : fileLength - 1;

            var contentLength = end - start + 1;

            Response.StatusCode = 206; // Partial Content
            Response.Headers.Add("Accept-Ranges", "bytes");
            Response.Headers.Add("Content-Range", $"bytes {start}-{end}/{fileLength}");
            Response.Headers.Add("Content-Length", contentLength.ToString());

            fileStream.Seek(start, SeekOrigin.Begin);
            var buffer = new byte[contentLength];
            await fileStream.ReadAsync(buffer, 0, (int)contentLength);

            return File(buffer, contentType);
        }
        // 7. Return Base64 encoded file (useful for APIs)
        [HttpGet("base64File/{FID:int}")]
        public IActionResult GetFileAsBase64(int FID)
        {



            string filePath = GetFileName(FID);

            string filename = System.IO.Path.GetFileName(filePath);

            if (string.IsNullOrEmpty(filePath))

            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var base64String = Convert.ToBase64String(fileBytes);
            var contentType = Utility.GetContentType(filename);

            return Ok(new
            {
                FileName = filename,
                ContentType = contentType,
                FileSize = fileBytes.Length,
                Base64Data = base64String
            });
        }

    }


}