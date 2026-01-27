using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;

namespace HyteraAPI.Services;

public class AssetService : IAssetService
{
    private readonly IDatabaseService _databaseService;
    private readonly string _rootPath;

    public AssetService(IDatabaseService databaseService, IConfiguration configuration)
    {
        _databaseService = databaseService;
        _rootPath = configuration["APISettings:DocVaultAssetsRoot"] ?? "D:\\Docvault\\www";
    }

    public async Task<(string FullPath, string ShowName)?> GetFileInfoAsync(int fileId)
    {
        var parameters = new Dictionary<string, object> { { "FID", fileId } };
        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_V2_Get_User_File_Info", parameters);

        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            var row = dataSet.Tables[0].Rows[0];
            var showAs = row["Show_Name"]?.ToString() ?? "";
            var phyPath = row["Phy_Path"]?.ToString() ?? "";
            var phyName = row["Phy_Name"]?.ToString() ?? "";
            var fullName = Path.Combine(Path.Combine(_rootPath, phyPath), phyName);

            if (File.Exists(fullName))
            {
                return (fullName, showAs);
            }
        }

        return null;
    }

    public string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
        {
            // Documents
            ".pdf" => "application/pdf",
            ".txt" => "text/plain",
            ".csv" => "text/csv",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

            // Images
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",

            // Audio/Video
            ".mp4" => "video/mp4",
            ".mp3" => "audio/mpeg",
            ".wav" => "audio/wav",
            ".webm" => "video/webm",

            // Archives
            ".zip" => "application/zip",
            ".rar" => "application/x-rar-compressed",

            // Web files
            ".html" => "text/html",
            ".css" => "text/css",
            ".js" => "application/javascript",
            ".json" => "application/json",

            // Default
            _ => "application/octet-stream"
        };
    }

    public async Task<byte[]> ResizeImageAsync(string filePath, int width, int height)
    {
        using var image = await Image.LoadAsync(filePath);

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Max
        }));

        using var ms = new MemoryStream();
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        if (extension == ".png")
        {
            await image.SaveAsync(ms, new PngEncoder());
        }
        else
        {
            await image.SaveAsync(ms, new JpegEncoder { Quality = 85 });
        }

        return ms.ToArray();
    }

    public async Task<byte[]> GetOriginalImageAsync(string filePath)
    {
        return await File.ReadAllBytesAsync(filePath);
    }
}
