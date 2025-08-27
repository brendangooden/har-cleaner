using HarCleaner.Models;

namespace HarCleaner.Filters;

public class HeaderFilter : IFilter
{
    private readonly string[] _includeHeaders;
    private readonly string[] _excludeHeaders;

    public string FilterName => "Header";

    public HeaderFilter(string[] includeHeaders, string[] excludeHeaders)
    {
        _includeHeaders = includeHeaders;
        _excludeHeaders = excludeHeaders;
    }

    public bool ShouldInclude(HarEntry entry)
    {
        // Filter request headers
        entry.Request.Headers = FilterHeaders(entry.Request.Headers);
        // Filter response headers
        entry.Response.Headers = FilterHeaders(entry.Response.Headers);
        return true;
    }

    private List<HarNameValuePair> FilterHeaders(List<HarNameValuePair> headers)
    {
        if (_includeHeaders.Length > 0)
        {
            // Only keep headers that match include list
            return headers.Where(h => _includeHeaders.Any(pattern => h.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToList();
        }
        if (_excludeHeaders.Length > 0)
        {
            // Remove headers that match exclude list
            return headers.Where(h => !_excludeHeaders.Any(pattern => h.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToList();
        }
        return headers;
    }
}
