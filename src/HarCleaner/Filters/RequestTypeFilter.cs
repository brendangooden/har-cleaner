using HarCleaner.Models;

namespace HarCleaner.Filters;

public class RequestTypeFilter : IFilter
{
    private readonly string[] _includeTypes;
    private readonly string[] _excludeTypes;

    public string FilterName => "RequestType";

    public RequestTypeFilter(string[] includeTypes, string[] excludeTypes)
    {
        _includeTypes = includeTypes.Select(t => t.ToLowerInvariant()).ToArray();
        _excludeTypes = excludeTypes.Select(t => t.ToLowerInvariant()).ToArray();
    }

    public bool ShouldInclude(HarEntry entry)
    {
        var url = entry.Request.Url;
        var mimeType = entry.Response.Content.MimeType?.ToLowerInvariant() ?? "";
        
        // Extract file extension from URL
        var fileExtension = GetFileExtension(url);
        
        // Check if we should exclude based on file type
        if (_excludeTypes.Length > 0)
        {
            if (IsTypeMatch(fileExtension, mimeType, _excludeTypes))
                return false;
        }

        // Check if we should include only specific types
        if (_includeTypes.Length > 0)
        {
            return IsTypeMatch(fileExtension, mimeType, _includeTypes);
        }

        return true;
    }

    private static string GetFileExtension(string url)
    {
        try
        {
            var uri = new Uri(url);
            var path = uri.AbsolutePath;
            var lastDot = path.LastIndexOf('.');
            var lastSlash = path.LastIndexOf('/');
            
            if (lastDot > lastSlash && lastDot < path.Length - 1)
            {
                return path.Substring(lastDot + 1).ToLowerInvariant();
            }
        }
        catch
        {
            // Ignore URI parsing errors
        }
        
        return string.Empty;
    }

    private static bool IsTypeMatch(string fileExtension, string mimeType, string[] types)
    {
        foreach (var type in types)
        {
            // Check file extension
            if (!string.IsNullOrEmpty(fileExtension) && fileExtension.Equals(type, StringComparison.OrdinalIgnoreCase))
                return true;

            // Check MIME type
            if (!string.IsNullOrEmpty(mimeType))
            {
                // Handle common mappings
                var isMatch = type switch
                {
                    "js" => mimeType.Contains("javascript") || mimeType.Contains("js"),
                    "css" => mimeType.Contains("css"),
                    "html" => mimeType.Contains("html"),
                    "json" => mimeType.Contains("json"),
                    "xml" => mimeType.Contains("xml"),
                    "png" => mimeType.Contains("png"),
                    "jpg" or "jpeg" => mimeType.Contains("jpeg"),
                    "gif" => mimeType.Contains("gif"),
                    "svg" => mimeType.Contains("svg"),
                    "pdf" => mimeType.Contains("pdf"),
                    _ => mimeType.Contains(type)
                };

                if (isMatch)
                    return true;
            }
        }

        return false;
    }
}
