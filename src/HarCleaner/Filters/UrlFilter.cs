using HarCleaner.Models;

namespace HarCleaner.Filters;

public class UrlFilter : IFilter
{
    private readonly string[] _includePatterns;
    private readonly string[] _excludePatterns;

    public string FilterName => "Url";

    public UrlFilter(string[] includePatterns, string[] excludePatterns)
    {
        _includePatterns = includePatterns;
        _excludePatterns = excludePatterns;
    }

    public bool ShouldInclude(HarEntry entry)
    {
        var url = entry.Request.Url;

        // Check excluded patterns
        if (_excludePatterns.Length > 0)
        {
            foreach (var pattern in _excludePatterns)
            {
                if (url.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
        }

        // Check included patterns
        if (_includePatterns.Length > 0)
        {
            foreach (var pattern in _includePatterns)
            {
                if (url.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false; // None of the include patterns matched
        }

        return true;
    }
}
