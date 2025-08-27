using System.Text.Json.Serialization;

namespace HarCleaner.Models;

public class HarEntry
{
	[JsonPropertyName("pageref")]
	public string? PageRef { get; set; }

	[JsonPropertyName("startedDateTime")]
	public DateTime StartedDateTime { get; set; }

	[JsonPropertyName("time")]
	public double Time { get; set; }

	[JsonPropertyName("request")]
	public HarRequest Request { get; set; } = new();

	[JsonPropertyName("response")]
	public HarResponse Response { get; set; } = new();

	[JsonPropertyName("cache")]
	public HarCache Cache { get; set; } = new();

	[JsonPropertyName("timings")]
	public HarTimings Timings { get; set; } = new();

	[JsonPropertyName("serverIPAddress")]
	public string? ServerIPAddress { get; set; }

	[JsonPropertyName("connection")]
	public string? Connection { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }

	// Chrome DevTools specific fields
	[JsonPropertyName("_resourceType")]
	public string? ResourceType { get; set; }

	[JsonPropertyName("_initiator")]
	public object? Initiator { get; set; }

	[JsonPropertyName("_priority")]
	public string? Priority { get; set; }

	[JsonPropertyName("_connectionId")]
	public string? ConnectionId { get; set; }
}

public class HarRequest
{
	[JsonPropertyName("method")]
	public string Method { get; set; } = string.Empty;

	[JsonPropertyName("url")]
	public string Url { get; set; } = string.Empty;

	[JsonPropertyName("httpVersion")]
	public string HttpVersion { get; set; } = string.Empty;

	[JsonPropertyName("headers")]
	public List<HarNameValuePair> Headers { get; set; } = new();

	[JsonPropertyName("queryString")]
	public List<HarNameValuePair> QueryString { get; set; } = new();

	[JsonPropertyName("cookies")]
	public List<HarCookie> Cookies { get; set; } = new();

	[JsonPropertyName("headersSize")]
	public long HeadersSize { get; set; }

	[JsonPropertyName("bodySize")]
	public long BodySize { get; set; }

	[JsonPropertyName("postData")]
	public HarPostData? PostData { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}

public class HarResponse
{
	[JsonPropertyName("status")]
	public int Status { get; set; }

	[JsonPropertyName("statusText")]
	public string StatusText { get; set; } = string.Empty;

	[JsonPropertyName("httpVersion")]
	public string HttpVersion { get; set; } = string.Empty;

	[JsonPropertyName("headers")]
	public List<HarNameValuePair> Headers { get; set; } = new();

	[JsonPropertyName("cookies")]
	public List<HarCookie> Cookies { get; set; } = new();

	[JsonPropertyName("content")]
	public HarContent Content { get; set; } = new();

	[JsonPropertyName("redirectURL")]
	public string RedirectURL { get; set; } = string.Empty;

	[JsonPropertyName("headersSize")]
	public long HeadersSize { get; set; }

	[JsonPropertyName("bodySize")]
	public long BodySize { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}

public class HarNameValuePair
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("value")]
	public string Value { get; set; } = string.Empty;

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}

public class HarCookie
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("value")]
	public string Value { get; set; } = string.Empty;

	[JsonPropertyName("path")]
	public string? Path { get; set; }

	[JsonPropertyName("domain")]
	public string? Domain { get; set; }

	[JsonPropertyName("expires")]
	public DateTime? Expires { get; set; }

	[JsonPropertyName("httpOnly")]
	public bool? HttpOnly { get; set; }

	[JsonPropertyName("secure")]
	public bool? Secure { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}

public class HarPostData
{
	[JsonPropertyName("mimeType")]
	public string MimeType { get; set; } = string.Empty;

	[JsonPropertyName("text")]
	public string? Text { get; set; }

	[JsonPropertyName("params")]
	public List<HarParam> Params { get; set; } = new();

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}

public class HarParam
{
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	[JsonPropertyName("value")]
	public string? Value { get; set; }

	[JsonPropertyName("fileName")]
	public string? FileName { get; set; }

	[JsonPropertyName("contentType")]
	public string? ContentType { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}

public class HarContent
{
	[JsonPropertyName("size")]
	public long Size { get; set; }

	[JsonPropertyName("compression")]
	public long? Compression { get; set; }

	[JsonPropertyName("mimeType")]
	public string MimeType { get; set; } = string.Empty;

	[JsonPropertyName("text")]
	public string? Text { get; set; }

	[JsonPropertyName("encoding")]
	public string? Encoding { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}

public class HarCache
{
	[JsonPropertyName("beforeRequest")]
	public HarCacheState? BeforeRequest { get; set; }

	[JsonPropertyName("afterRequest")]
	public HarCacheState? AfterRequest { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}

public class HarCacheState
{
	[JsonPropertyName("expires")]
	public DateTime? Expires { get; set; }

	[JsonPropertyName("lastAccess")]
	public DateTime LastAccess { get; set; }

	[JsonPropertyName("eTag")]
	public string ETag { get; set; } = string.Empty;

	[JsonPropertyName("hitCount")]
	public int HitCount { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}

public class HarTimings
{
	[JsonPropertyName("blocked")]
	public double? Blocked { get; set; }

	[JsonPropertyName("dns")]
	public double? Dns { get; set; }

	[JsonPropertyName("connect")]
	public double? Connect { get; set; }

	[JsonPropertyName("send")]
	public double Send { get; set; }

	[JsonPropertyName("wait")]
	public double Wait { get; set; }

	[JsonPropertyName("receive")]
	public double Receive { get; set; }

	[JsonPropertyName("ssl")]
	public double? Ssl { get; set; }

	[JsonPropertyName("comment")]
	public string? Comment { get; set; }
}
