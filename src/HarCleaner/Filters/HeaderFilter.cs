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
        var filteredHeaders = headers;

        // First apply include filter if specified
        if (_includeHeaders.Length > 0)
        {
            filteredHeaders = filteredHeaders.Where(h => _includeHeaders.Any(pattern => h.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        // Then apply exclude filter if specified
        if (_excludeHeaders.Length > 0)
        {
            filteredHeaders = filteredHeaders.Where(h => !_excludeHeaders.Any(pattern => h.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        return filteredHeaders;
    }
}
