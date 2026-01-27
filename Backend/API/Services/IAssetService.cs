namespace HyteraAPI.Services;

public interface IAssetService
{
    Task<(string FullPath, string ShowName)?> GetFileInfoAsync(int fileId);
    string GetContentType(string fileName);
    Task<byte[]> ResizeImageAsync(string filePath, int width, int height);
    Task<byte[]> GetOriginalImageAsync(string filePath);
}
