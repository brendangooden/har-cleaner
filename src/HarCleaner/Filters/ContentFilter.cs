using HarCleaner.Models;

namespace HarCleaner.Filters;

public class ContentFilter : IFilter
{
	private readonly bool _removeResponseContent;
	private readonly bool _removeRequestContent;
	private readonly bool _removeBase64Content;
	private readonly long? _maxContentSize;
	private readonly string[] _excludeContentTypes;

	public string FilterName => "Content";

	public ContentFilter(bool removeResponseContent = false, bool removeRequestContent = false,
						bool removeBase64Content = false, long? maxContentSize = null,
						string[]? excludeContentTypes = null)
	{
		_removeResponseContent = removeResponseContent;
		_removeRequestContent = removeRequestContent;
		_removeBase64Content = removeBase64Content;
		_maxContentSize = maxContentSize;
		_excludeContentTypes = excludeContentTypes ?? Array.Empty<string>();
	}

	public bool ShouldInclude(HarEntry entry)
	{
		// Clean response content
		if (_removeResponseContent)
		{
			entry.Response.Content.Text = null;
		}

		// Clean request content (POST data)
		if (_removeRequestContent && entry.Request.PostData != null)
		{
			entry.Request.PostData.Text = null;
			entry.Request.PostData.Params.Clear();
		}

		// Remove large content
		if (_maxContentSize.HasValue && entry.Response.Content.Size > _maxContentSize.Value)
		{
			entry.Response.Content.Text = $"[CONTENT REMOVED - Size: {entry.Response.Content.Size} bytes]";
		}

		// Remove specific content types
		if (_excludeContentTypes.Length > 0)
		{
			var mimeType = entry.Response.Content.MimeType?.ToLowerInvariant() ?? "";
			if (_excludeContentTypes.Any(ct => mimeType.Contains(ct.ToLowerInvariant())))
			{
				entry.Response.Content.Text = $"[CONTENT REMOVED - Type: {entry.Response.Content.MimeType}]";
			}
		}

		// Remove base64 encoded content
		if (_removeBase64Content && !string.IsNullOrEmpty(entry.Response.Content.Text))
		{
			if (IsBase64Content(entry.Response.Content.Text))
			{
				entry.Response.Content.Text = "[BASE64 CONTENT REMOVED]";
			}
		}

		return true;
	}

	private static bool IsBase64Content(string content)
	{
		if (string.IsNullOrEmpty(content) || content.Length < 50)
		{
			return false;
		}

		// Check if content looks like base64
		return content.All(c => char.IsLetterOrDigit(c) || c == '+' || c == '/' || c == '=') &&
			   content.Length % 4 == 0;
	}
}
