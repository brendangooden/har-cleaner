using System.Text.Json.Serialization;

namespace HarCleaner.Models;

public class MlIngestRequest
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("query_params")]
    public string QueryParams { get; set; } = string.Empty;

    [JsonPropertyName("headers")]
    public string Headers { get; set; } = string.Empty;

    [JsonPropertyName("cookies")]
    public string Cookies { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("user_agent_category")]
    public string UserAgentCategory { get; set; } = string.Empty;

    [JsonPropertyName("has_auth")]
    public bool HasAuth { get; set; }
}

public class MlIngestResponse
{
    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("headers")]
    public string Headers { get; set; } = string.Empty;

    [JsonPropertyName("cookies")]
    public string Cookies { get; set; } = string.Empty;

    [JsonPropertyName("body")]
    public string? Body { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("content_type")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; } = string.Empty;

    [JsonPropertyName("cache_status")]
    public string CacheStatus { get; set; } = string.Empty;
}

public class MlIngestEntry
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("response_time_ms")]
    public double ResponseTimeMs { get; set; }

    [JsonPropertyName("request")]
    public MlIngestRequest Request { get; set; } = new();

    [JsonPropertyName("response")]
    public MlIngestResponse Response { get; set; } = new();

    [JsonPropertyName("request_type")]
    public string RequestType { get; set; } = string.Empty;

    [JsonPropertyName("resource_type")]
    public string ResourceType { get; set; } = string.Empty;
}
