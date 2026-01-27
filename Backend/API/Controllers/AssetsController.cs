using HyteraAPI.Models.Responses;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Controllers;

[Route("Asset")]
[ApiController]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;
    private readonly IConfiguration _configuration;

    public AssetsController(IAssetService assetService, IConfiguration configuration)
    {
        _assetService = assetService;
        _configuration = configuration;
    }

    [HttpGet("Image/{imageId:int}/{width:int?}/{height:int?}")]
    public async Task<IActionResult> GetImage(int imageId, int width = 800, int height = 600)
    {
        var rootPath = _configuration["APISettings:DocVaultAssetsRoot"] ?? "D:\\Docvault\\www";
        var fileInfo = await _assetService.GetFileInfoAsync(imageId);

        string fullName;
        string showAs;

        if (fileInfo == null)
        {
            showAs = "NotFound.png";
            fullName = Path.Combine(rootPath, showAs);

            if (!System.IO.File.Exists(fullName))
            {
                return NotFound("Image not found");
            }
        }
        else
        {
            fullName = fileInfo.Value.FullPath;
            showAs = Path.GetFileName(fullName);
        }

        byte[] imageBytes;
        string contentType;

        if (width <= 0 || height <= 0)
        {
            imageBytes = await _assetService.GetOriginalImageAsync(fullName);
        }
        else
        {
            imageBytes = await _assetService.ResizeImageAsync(fullName, width, height);
        }

        var extension = Path.GetExtension(fullName).ToLowerInvariant();
        contentType = extension == ".png" ? "image/png" : "image/jpeg";

        return File(imageBytes, contentType);
    }

    [HttpGet("File/{fileId:int}")]
    public async Task<IActionResult> GetFile(int fileId)
    {
        var fileInfo = await _assetService.GetFileInfoAsync(fileId);

        if (fileInfo == null)
        {
            return NotFound();
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(fileInfo.Value.FullPath);
        var contentType = _assetService.GetContentType(fileInfo.Value.ShowName);

        return File(fileBytes, contentType, fileInfo.Value.ShowName);
    }

    [HttpGet("viewFile/{fileId:int}")]
    public async Task<IActionResult> ViewFile(int fileId)
    {
        var fileInfo = await _assetService.GetFileInfoAsync(fileId);

        if (fileInfo == null)
        {
            return NotFound();
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(fileInfo.Value.FullPath);
        var contentType = _assetService.GetContentType(fileInfo.Value.ShowName);

        Response.Headers.Append("Content-Disposition", "inline");
        return File(fileBytes, contentType);
    }

    [HttpGet("streamvideo/{fileId:int}")]
    public async Task<IActionResult> StreamVideo(int fileId)
    {
        var fileInfo = await _assetService.GetFileInfoAsync(fileId);

        if (fileInfo == null)
        {
            return NotFound();
        }

        var filePath = fileInfo.Value.FullPath;
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var fileLength = new FileInfo(filePath).Length;

        var rangeHeader = Request.Headers["Range"].ToString();
        if (!string.IsNullOrEmpty(rangeHeader))
        {
            return await HandleRangeRequestAsync(fileStream, rangeHeader, "video/mp4", fileLength);
        }

        return File(fileStream, "video/mp4", enableRangeProcessing: true);
    }

    [HttpGet("base64File/{fileId:int}")]
    public async Task<IActionResult> GetFileAsBase64(int fileId)
    {
        var fileInfo = await _assetService.GetFileInfoAsync(fileId);

        if (fileInfo == null)
        {
            return NotFound();
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(fileInfo.Value.FullPath);
        var base64String = Convert.ToBase64String(fileBytes);
        var contentType = _assetService.GetContentType(fileInfo.Value.ShowName);

        return Ok(new FileBase64Response
        {
            FileName = fileInfo.Value.ShowName,
            ContentType = contentType,
            FileSize = fileBytes.Length,
            Base64Data = base64String
        });
    }

    private async Task<IActionResult> HandleRangeRequestAsync(
        FileStream fileStream,
        string rangeHeader,
        string contentType,
        long fileLength)
    {
        var ranges = rangeHeader.Replace("bytes=", "").Split('-');
        var start = long.Parse(ranges[0]);
        var end = ranges.Length > 1 && !string.IsNullOrEmpty(ranges[1])
            ? long.Parse(ranges[1])
            : fileLength - 1;

        var contentLength = end - start + 1;

        Response.StatusCode = 206;
        Response.Headers.Append("Accept-Ranges", "bytes");
        Response.Headers.Append("Content-Range", $"bytes {start}-{end}/{fileLength}");
        Response.Headers.Append("Content-Length", contentLength.ToString());

        fileStream.Seek(start, SeekOrigin.Begin);
        var buffer = new byte[contentLength];
        await fileStream.ReadAsync(buffer, 0, (int)contentLength);

        return File(buffer, contentType);
    }
}
