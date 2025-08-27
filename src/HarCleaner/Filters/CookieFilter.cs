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
        if (_includeCookies.Length > 0)
        {
            // Only keep cookies that match include list
            return cookies.Where(c => _includeCookies.Any(pattern => c.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToList();
        }
        if (_excludeCookies.Length > 0)
        {
            // Remove cookies that match exclude list
            return cookies.Where(c => !_excludeCookies.Any(pattern => c.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))).ToList();
        }
        return cookies;
    }
}
