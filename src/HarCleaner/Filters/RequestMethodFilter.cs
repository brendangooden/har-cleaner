using HarCleaner.Models;

namespace HarCleaner.Filters;

public class RequestMethodFilter : IFilter
{
	private readonly bool _xhrOnly;
	private readonly string[] _includeMethods;
	private readonly string[] _excludeMethods;

	public string FilterName => "RequestMethod";

	public RequestMethodFilter(bool xhrOnly, string[] includeMethods, string[] excludeMethods)
	{
		_xhrOnly = xhrOnly;
		_includeMethods = includeMethods.Select(m => m.ToUpperInvariant()).ToArray();
		_excludeMethods = excludeMethods.Select(m => m.ToUpperInvariant()).ToArray();
	}

	public bool ShouldInclude(HarEntry entry)
	{
		var method = entry.Request.Method.ToUpperInvariant();

		// Check XHR/AJAX only filter
		if (_xhrOnly && !IsXhrRequest(entry))
		{
			return false;
		}

		// Check excluded methods
		if (_excludeMethods.Length > 0 && _excludeMethods.Contains(method))
		{
			return false;
		}

		// Check included methods
		if (_includeMethods.Length > 0 && !_includeMethods.Contains(method))
		{
			return false;
		}

		return true;
	}

	private static bool IsXhrRequest(HarEntry entry)
	{
		// Check for XMLHttpRequest headers
		var headers = entry.Request.Headers;

		// Look for X-Requested-With header (common XHR indicator)
		var xRequestedWith = headers.FirstOrDefault(h =>
			h.Name.Equals("X-Requested-With", StringComparison.OrdinalIgnoreCase));
		if (xRequestedWith != null && xRequestedWith.Value.Contains("XMLHttpRequest", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		// Check Accept header for JSON/XML (common for AJAX)
		var acceptHeader = headers.FirstOrDefault(h =>
			h.Name.Equals("Accept", StringComparison.OrdinalIgnoreCase));
		if (acceptHeader != null)
		{
			var accept = acceptHeader.Value.ToLowerInvariant();
			if (accept.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
				accept.Contains("application/xml", StringComparison.OrdinalIgnoreCase) ||
				accept.Contains("text/xml", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		// Check Content-Type for JSON (common for AJAX POST requests)
		var contentTypeHeader = headers.FirstOrDefault(h =>
			h.Name.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
		if (contentTypeHeader != null)
		{
			var contentType = contentTypeHeader.Value.ToLowerInvariant();
			if (contentType.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
				contentType.Contains("application/xml", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		// Check if response content type suggests API call
		var responseContentType = entry.Response.Content.MimeType?.ToLowerInvariant() ?? "";
		if (responseContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase) ||
			responseContentType.Contains("application/xml", StringComparison.OrdinalIgnoreCase) ||
			responseContentType.Contains("text/xml", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		return false;
	}
}
