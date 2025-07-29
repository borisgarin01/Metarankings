namespace FilesManagement;

public sealed class ContentTypeGetter
{
    public string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            ".tiff" => "image/tiff",
            _ => "application/octet-stream" // default for unknown types
        };
    }
}
