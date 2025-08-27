using HarCleaner.Models;

namespace HarCleaner.Tests.Helpers;

public static class TestDataHelper
{
	public static HarFile CreateTestHarFile()
	{
		return new HarFile
		{
			Log = new HarLog
			{
				Version = "1.2",
				Creator = new HarCreator { Name = "Test", Version = "1.0" },
				Pages = new List<HarPage>
				{
					new HarPage
					{
						Id = "page_1",
						Title = "Test Page",
						StartedDateTime = DateTime.UtcNow,
						PageTimings = new HarPageTimings { OnLoad = 1000 }
					}
				},
				Entries = new List<HarEntry>()
			}
		};
	}

	public static HarEntry CreateTestEntry(string url, string method = "GET", int status = 200,
		string mimeType = "text/html", long size = 1000,
		List<HarNameValuePair>? requestHeaders = null, 
		List<HarCookie>? requestCookies = null,
		List<HarNameValuePair>? responseHeaders = null,
		List<HarCookie>? responseCookies = null
		)
	{
		return new HarEntry
		{
			StartedDateTime = DateTime.UtcNow,
			Time = 100,
			Request = new HarRequest
			{
				Method = method,
				Url = url,
				HttpVersion = "HTTP/1.1",
				Headers = requestHeaders ?? new List<HarNameValuePair>(),
				Cookies = requestCookies ?? new List<HarCookie>(),
				QueryString = new List<HarNameValuePair>(),
				HeadersSize = 200,
				BodySize = 0
			},
			Response = new HarResponse
			{
				Status = status,
				StatusText = GetStatusText(status),
				HttpVersion = "HTTP/1.1",
                Headers = responseHeaders ?? new List<HarNameValuePair>(),
                Cookies = responseCookies ?? new List<HarCookie>(),
                Content = new HarContent
				{
					Size = size,
					MimeType = mimeType
				},
				RedirectURL = "",
				HeadersSize = 150,
				BodySize = size
			},
			Cache = new HarCache(),
			Timings = new HarTimings
			{
				Send = 10,
				Wait = 80,
				Receive = 10
			}
		};
	}

	public static HarEntry CreateXhrEntry(string url)
	{
		var headers = new List<HarNameValuePair>
		{
			new HarNameValuePair { Name = "Accept", Value = "application/json" },
			new HarNameValuePair { Name = "Content-Type", Value = "application/json" },
			new HarNameValuePair { Name = "X-Requested-With", Value = "XMLHttpRequest" }
		};

		return CreateTestEntry(url, "POST", 200, "application/json", 500, headers);
	}

	public static HarEntry CreateStaticAssetEntry(string url, string fileType)
	{
		var mimeType = fileType switch
		{
			"js" => "application/javascript",
			"css" => "text/css",
			"png" => "image/png",
			"jpg" => "image/jpeg",
			_ => "text/plain"
		};

		return CreateTestEntry(url, "GET", 200, mimeType, 2000);
	}

	public static HarEntry CreateEntryWithCookies(string url)
	{
		var cookies = new List<HarCookie>
		{
			new HarCookie { Name = "session_id", Value = "abc123", HttpOnly = true },
			new HarCookie { Name = "_ga", Value = "GA1.1.123456789", Domain = ".example.com" },
			new HarCookie { Name = "auth_token", Value = "bearer_token_123", Secure = true }
		};

		var headers = new List<HarNameValuePair>
		{
			new HarNameValuePair { Name = "Cookie", Value = "session_id=abc123; _ga=GA1.1.123456789; auth_token=bearer_token_123" },
			new HarNameValuePair { Name = "Authorization", Value = "Bearer token123" }
		};

        // Use same cookies and headers for request and response for testing
        return CreateTestEntry(url, "GET", 200, "text/html", 1000, headers, cookies, headers, cookies);
	}

	public static HarEntry CreateEntryWithContent(string url, string content, string mimeType = "text/html")
	{
		var entry = CreateTestEntry(url, "GET", 200, mimeType, content.Length);
		entry.Response.Content.Text = content;
		return entry;
	}

	private static string GetStatusText(int status)
	{
		return status switch
		{
			200 => "OK",
			302 => "Found",
			404 => "Not Found",
			500 => "Internal Server Error",
			_ => "Unknown"
		};
	}
}
