using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using HarCleaner.Models;

namespace HarCleaner.Services;

public class MlIngestExporter
{
    private static readonly string[] ApiIndicators = { "/api/", "/v1/", "/v2/", "/v3/", "/rest/", "/graphql" };
    private static readonly string[] StaticExtensions = { ".js", ".css", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".woff", ".woff2", ".ttf", ".eot" };

    public async Task SaveAsync(HarFile harFile, string filePath)
    {
        try
        {
            var mlEntries = ConvertToMlIngestFormat(harFile);
            
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonContent = JsonSerializer.Serialize(mlEntries, options);
            
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(filePath, jsonContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save ML-ingest file: {ex.Message}", ex);
        }
    }

    public void Save(HarFile harFile, string filePath)
    {
        SaveAsync(harFile, filePath).GetAwaiter().GetResult();
    }

    private List<MlIngestEntry> ConvertToMlIngestFormat(HarFile harFile)
    {
        var mlEntries = new List<MlIngestEntry>();

        foreach (var entry in harFile.Log.Entries)
        {
            try
            {
                var mlEntry = new MlIngestEntry
                {
                    Timestamp = entry.StartedDateTime,
                    Method = entry.Request.Method,
                    FullUrl = entry.Request.Url,
                    StatusCode = entry.Response.Status,
                    ResponseTimeMs = entry.Time,
                    RequestSize = CalculateRequestSize(entry),
                    ResponseSize = CalculateResponseSize(entry),
                    ContentType = GetContentType(entry),
                    Cookies = CombineCookies(entry),
                    Headers = CombineHeaders(entry),
                    QueryParams = ExtractQueryParams(entry),
                    RequestBody = GetRequestBody(entry),
                    ResponseBody = GetResponseBody(entry),
                    MimeType = GetMimeType(entry),
                    CacheStatus = GetCacheStatus(entry),
                    ResourceType = entry.ResourceType ?? string.Empty
                };

                // Extract domain and path
                if (Uri.TryCreate(entry.Request.Url, UriKind.Absolute, out var uri))
                {
                    mlEntry.Domain = uri.Host;
                    mlEntry.Path = uri.AbsolutePath;
                }

                // Set derived fields
                mlEntry.RequestType = DetermineRequestType(entry);
                mlEntry.HasAuth = HasAuthHeaders(entry);
                mlEntry.UserAgentCategory = CategorizeUserAgent(entry);

                mlEntries.Add(mlEntry);
            }
            catch (Exception ex)
            {
                // Log error but continue processing other entries
                Console.WriteLine($"Warning: Failed to process entry {entry.Request.Url}: {ex.Message}");
            }
        }

        return mlEntries;
    }

    private long CalculateRequestSize(HarEntry entry)
    {
        long size = entry.Request.HeadersSize;
        size += entry.Request.BodySize;
        return size;
    }

    private long CalculateResponseSize(HarEntry entry)
    {
        long size = entry.Response.HeadersSize;
        size += entry.Response.BodySize;
        size += entry.Response.Content?.Size ?? 0;
        return size;
    }

    private string GetContentType(HarEntry entry)
    {
        var contentTypeHeader = entry.Response.Headers
            .FirstOrDefault(h => h.Name.Equals("content-type", StringComparison.OrdinalIgnoreCase));
        
        if (contentTypeHeader != null)
        {
            // Extract just the main content type (before semicolon)
            var contentType = contentTypeHeader.Value.Split(';')[0].Trim().ToLowerInvariant();
            return contentType;
        }

        return string.Empty;
    }

    private string GetMimeType(HarEntry entry)
    {
        return entry.Response.Content?.MimeType ?? string.Empty;
    }

    private string CombineCookies(HarEntry entry)
    {
        var cookies = new List<string>();
        
        // Request cookies
        if (entry.Request.Cookies?.Count > 0)
        {
            cookies.AddRange(entry.Request.Cookies.Select(c => $"{c.Name}={c.Value}"));
        }

        // Response set-cookie headers
        var setCookieHeaders = entry.Response.Headers
            .Where(h => h.Name.Equals("set-cookie", StringComparison.OrdinalIgnoreCase))
            .Select(h => h.Value);
        
        cookies.AddRange(setCookieHeaders);

        return string.Join("; ", cookies);
    }

    private string CombineHeaders(HarEntry entry)
    {
        var headers = new List<string>();
        
        // Request headers (excluding cookies and auth for brevity)
        var requestHeaders = entry.Request.Headers
            .Where(h => !h.Name.Equals("cookie", StringComparison.OrdinalIgnoreCase) &&
                       !h.Name.Equals("authorization", StringComparison.OrdinalIgnoreCase))
            .Select(h => $"{h.Name}={h.Value}");
        
        headers.AddRange(requestHeaders);

        return string.Join("; ", headers);
    }

    private string ExtractQueryParams(HarEntry entry)
    {
        var queryParams = new List<string>();

        // Get from URL
        if (Uri.TryCreate(entry.Request.Url, UriKind.Absolute, out var uri) && !string.IsNullOrEmpty(uri.Query))
        {
            queryParams.Add(uri.Query.TrimStart('?'));
        }

        // Get from HAR queryString array
        if (entry.Request.QueryString?.Count > 0)
        {
            var harQueryParams = entry.Request.QueryString
                .Select(q => $"{q.Name}={q.Value}");
            queryParams.AddRange(harQueryParams);
        }

        return string.Join("&", queryParams.Distinct());
    }

    private string? GetRequestBody(HarEntry entry)
    {
        if (entry.Request.PostData?.Text != null)
        {
            // Limit size to prevent huge bodies
            const int maxBodySize = 10000;
            var body = entry.Request.PostData.Text;
            if (body.Length > maxBodySize)
            {
                return body.Substring(0, maxBodySize) + "... [truncated]";
            }
            return body;
        }
        return null;
    }

    private string? GetResponseBody(HarEntry entry)
    {
        if (entry.Response.Content?.Text != null)
        {
            // Limit size to prevent huge bodies
            const int maxBodySize = 10000;
            var body = entry.Response.Content.Text;
            if (body.Length > maxBodySize)
            {
                return body.Substring(0, maxBodySize) + "... [truncated]";
            }
            return body;
        }
        return null;
    }

    private string DetermineRequestType(HarEntry entry)
    {
        // First check Chrome DevTools resource type (most accurate)
        if (!string.IsNullOrEmpty(entry.ResourceType))
        {
            var resourceType = entry.ResourceType.ToLowerInvariant();
            
            // Map Chrome DevTools resource types to our categories
            return resourceType switch
            {
                "xhr" => "xhr",
                "fetch" => "fetch", 
                "document" => "document",
                "script" => "script",
                "stylesheet" => "stylesheet",
                "image" => "image",
                "media" => "media",
                "font" => "font",
                "websocket" => "websocket",
                "manifest" => "manifest",
                "other" => "other",
                _ => resourceType // Use the original if it's something we don't recognize
            };
        }

        // Fallback: Determine type based on headers and content
        if (IsXhrRequest(entry))
            return "xhr";

        // Check if it's an API call based on URL patterns
        if (IsApiCallByUrl(entry.Request.Url))
            return "api";

        // Check if it's a static resource based on URL
        if (IsStaticResourceByUrl(entry.Request.Url))
        {
            var url = entry.Request.Url.ToLowerInvariant();
            if (url.Contains(".js")) return "script";
            if (url.Contains(".css")) return "stylesheet";
            if (url.Contains(".png") || url.Contains(".jpg") || url.Contains(".jpeg") || url.Contains(".gif") || url.Contains(".svg") || url.Contains(".ico")) return "image";
            if (url.Contains(".woff") || url.Contains(".woff2") || url.Contains(".ttf") || url.Contains(".eot")) return "font";
            if (url.Contains(".mp4") || url.Contains(".mp3") || url.Contains(".wav") || url.Contains(".avi")) return "media";
            return "static";
        }

        // Default fallback
        return "other";
    }

    private bool IsApiCallByUrl(string url)
    {
        var lowerUrl = url.ToLowerInvariant();
        return ApiIndicators.Any(indicator => lowerUrl.Contains(indicator)) ||
               lowerUrl.Contains("api.") || // Check for api subdomain
               lowerUrl.Contains(".api.");   // Check for api in subdomain
    }

    private static bool IsXhrRequest(HarEntry entry)
    {
        // Check for XMLHttpRequest headers
        var headers = entry.Request.Headers;
        
        // Look for X-Requested-With header (common XHR indicator)
        var xRequestedWith = headers.FirstOrDefault(h => 
            h.Name.Equals("X-Requested-With", StringComparison.OrdinalIgnoreCase));
        if (xRequestedWith != null && xRequestedWith.Value.Contains("XMLHttpRequest"))
            return true;

        // Check Accept header for JSON/XML (common for AJAX)
        var acceptHeader = headers.FirstOrDefault(h => 
            h.Name.Equals("Accept", StringComparison.OrdinalIgnoreCase));
        if (acceptHeader != null)
        {
            var accept = acceptHeader.Value.ToLowerInvariant();
            if (accept.Contains("application/json") || 
                accept.Contains("application/xml") ||
                accept.Contains("text/xml"))
                return true;
        }

        // Check Content-Type for JSON (common for AJAX POST requests)
        var contentTypeHeader = headers.FirstOrDefault(h => 
            h.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
        if (contentTypeHeader != null)
        {
            var contentType = contentTypeHeader.Value.ToLowerInvariant();
            if (contentType.Contains("application/json") ||
                contentType.Contains("application/xml"))
                return true;
        }

        // Check if response content type suggests API call
        var responseContentType = entry.Response.Content.MimeType?.ToLowerInvariant() ?? "";
        if (responseContentType.Contains("application/json") ||
            responseContentType.Contains("application/xml") ||
            responseContentType.Contains("text/xml"))
            return true;

        return false;
    }

    private bool IsStaticResourceByUrl(string url)
    {
        var lowerUrl = url.ToLowerInvariant();
        return StaticExtensions.Any(ext => lowerUrl.Contains(ext));
    }

    private bool HasAuthHeaders(HarEntry entry)
    {
        return entry.Request.Headers.Any(h => 
            h.Name.Equals("authorization", StringComparison.OrdinalIgnoreCase) ||
            h.Name.Equals("x-api-key", StringComparison.OrdinalIgnoreCase) ||
            h.Name.Equals("x-auth-token", StringComparison.OrdinalIgnoreCase) ||
            h.Value.ToLowerInvariant().Contains("bearer") ||
            h.Value.ToLowerInvariant().Contains("token"));
    }

    private string CategorizeUserAgent(HarEntry entry)
    {
        var userAgentHeader = entry.Request.Headers
            .FirstOrDefault(h => h.Name.Equals("user-agent", StringComparison.OrdinalIgnoreCase));
        
        if (userAgentHeader == null)
            return "unknown";

        var ua = userAgentHeader.Value.ToLowerInvariant();
        
        if (ua.Contains("chrome")) return "chrome";
        if (ua.Contains("firefox")) return "firefox";
        if (ua.Contains("safari")) return "safari";
        if (ua.Contains("edge")) return "edge";
        if (ua.Contains("mobile")) return "mobile";
        if (ua.Contains("bot") || ua.Contains("crawler")) return "bot";
        
        return "other";
    }

    private string GetCacheStatus(HarEntry entry)
    {
        var cacheHeader = entry.Response.Headers
            .FirstOrDefault(h => h.Name.Equals("cache-control", StringComparison.OrdinalIgnoreCase));
        
        if (cacheHeader != null)
        {
            return cacheHeader.Value;
        }

        // Check if it came from cache
        if (entry.Cache != null && !string.IsNullOrEmpty(entry.Cache.Comment))
        {
            return entry.Cache.Comment;
        }

        return string.Empty;
    }
}
