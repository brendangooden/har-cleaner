using System.Text.Json.Serialization;

namespace HarCleaner.Models;

public class MlIngestEntry
{
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("domain")]
    public string Domain { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("full_url")]
    public string FullUrl { get; set; } = string.Empty;

    [JsonPropertyName("status_code")]
    public int StatusCode { get; set; }

    [JsonPropertyName("response_time_ms")]
    public double ResponseTimeMs { get; set; }

    [JsonPropertyName("request_size")]
    public long RequestSize { get; set; }

    [JsonPropertyName("response_size")]
    public long ResponseSize { get; set; }

    [JsonPropertyName("content_type")]
    public string ContentType { get; set; } = string.Empty;

    [JsonPropertyName("cookies")]
    public string Cookies { get; set; } = string.Empty;

    [JsonPropertyName("headers")]
    public string Headers { get; set; } = string.Empty;

    [JsonPropertyName("query_params")]
    public string QueryParams { get; set; } = string.Empty;

    [JsonPropertyName("request_body")]
    public string? RequestBody { get; set; }

    [JsonPropertyName("response_body")]
    public string? ResponseBody { get; set; }

    [JsonPropertyName("request_type")]
    public string RequestType { get; set; } = string.Empty;

    [JsonPropertyName("has_auth")]
    public bool HasAuth { get; set; }

    [JsonPropertyName("user_agent_category")]
    public string UserAgentCategory { get; set; } = string.Empty;

    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; } = string.Empty;

    [JsonPropertyName("cache_status")]
    public string CacheStatus { get; set; } = string.Empty;

    [JsonPropertyName("resource_type")]
    public string ResourceType { get; set; } = string.Empty;
}
