namespace FunTimePIE
{
 
    public static class Utility
    {

        private static readonly IConfiguration _configuration;
        static Utility()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
        public static string DefaultAssetRoot => _configuration["APISettings:DocVaultAssetsRoot"];
        public static string DefaultConnectionString => _configuration.GetConnectionString("HYTRestAPI");
         
        public static string GetSharedData()
        {
            return "This is shared data";
        }
        public static string GetClientIpAddress(HttpContext httpContext)
        {
            string ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            }

            return ipAddress;
        }


        public static string GetContentType(string fileName)
        {
            // Extract the file extension (e.g., ".pdf", ".jpg", ".txt")
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            // Map file extensions to MIME types
            return extension switch
            {
                // Documents
                ".pdf" => "application/pdf",           // PDF files
                ".txt" => "text/plain",               // Plain text
                ".csv" => "text/csv",                 // CSV files
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", // Excel
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document", // Word

                // Images
                ".jpg" or ".jpeg" => "image/jpeg",    // JPEG images
                ".png" => "image/png",                // PNG images
                ".gif" => "image/gif",                // GIF images
                ".svg" => "image/svg+xml",            // SVG images

                // Audio/Video
                ".mp4" => "video/mp4",                // MP4 videos
                ".mp3" => "audio/mpeg",               // MP3 audio
                ".wav" => "audio/wav",                // WAV audio

                // Archives
                ".zip" => "application/zip",          // ZIP files
                ".rar" => "application/x-rar-compressed", // RAR files

                // Web files
                ".html" => "text/html",               // HTML files
                ".css" => "text/css",                 // CSS files
                ".js" => "application/javascript",    // JavaScript files
                ".json" => "application/json",        // JSON files

                // Default for unknown types
                _ => "application/octet-stream"       // Generic binary file
            };
        }
    }
}
