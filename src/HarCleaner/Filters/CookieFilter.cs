using HarCleaner.Models;

namespace HarCleaner.Filters;

public class CookieFilter : IFilter
{
    private readonly string[] _includeCookies;
    private readonly string[] _excludeCookies;

    public string FilterName => "Cookie";

    public CookieFilter(string[] includeCookies, string[] excludeCookies)
    {
        _includeCookies = includeCookies;
        _excludeCookies = excludeCookies;
    }

    public bool ShouldInclude(HarEntry entry)
    {
        // Filter request cookies
        entry.Request.Cookies = FilterCookies(entry.Request.Cookies);
        // Filter response cookies
        entry.Response.Cookies = FilterCookies(entry.Response.Cookies);
        return true;
    }

    private List<HarCookie> FilterCookies(List<HarCookie> cookies)
    {
        var filteredCookies = cookies;

        // First apply include filter if specified
        if (_includeCookies.Length > 0)
        {
            filteredCookies = filteredCookies.Where(c => _includeCookies.Any(pattern => c.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        // Then apply exclude filter if specified
        if (_excludeCookies.Length > 0)
        {
            filteredCookies = filteredCookies.Where(c => !_excludeCookies.Any(pattern => c.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        return filteredCookies;
    }
}
