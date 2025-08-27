using HarCleaner.Models;

namespace HarCleaner.Filters;

public class PrivacyFilter : IFilter
{
    private readonly bool _removeCookies;
    private readonly bool _removeAuthTokens;
    private readonly bool _removePersonalIdentifiers;
    private readonly bool _removeTrackingHeaders;
    private readonly string[] _sensitiveHeaders;
    private readonly string[] _sensitiveParams;

    public string FilterName => "Privacy";

    public PrivacyFilter(bool removeCookies = false, bool removeAuthTokens = false, 
                        bool removePersonalIdentifiers = false, bool removeTrackingHeaders = false,
                        string[]? sensitiveHeaders = null, string[]? sensitiveParams = null)
    {
        _removeCookies = removeCookies;
        _removeAuthTokens = removeAuthTokens;
        _removePersonalIdentifiers = removePersonalIdentifiers;
        _removeTrackingHeaders = removeTrackingHeaders;
        
        _sensitiveHeaders = sensitiveHeaders ?? new[]
        {
            "authorization", "cookie", "set-cookie", "x-auth-token", "x-api-key",
            "x-session-id", "x-csrf-token", "x-requested-with"
        };
        
        _sensitiveParams = sensitiveParams ?? new[]
        {
            "token", "key", "session", "auth", "password", "secret", "api_key", "csrf"
        };
    }

    public bool ShouldInclude(HarEntry entry)
    {
        // This filter doesn't exclude entries, it cleans them
        if (_removeCookies)
        {
            entry.Request.Cookies.Clear();
            entry.Response.Cookies.Clear();
            RemoveHeadersByName(entry.Request.Headers, "cookie");
            RemoveHeadersByName(entry.Response.Headers, "set-cookie");
        }

        if (_removeAuthTokens || _removePersonalIdentifiers)
        {
            CleanHeaders(entry.Request.Headers);
            CleanHeaders(entry.Response.Headers);
            CleanQueryString(entry.Request.QueryString);
        }

        if (_removeTrackingHeaders)
        {
            RemoveTrackingHeaders(entry.Request.Headers);
            RemoveTrackingHeaders(entry.Response.Headers);
        }

        return true;
    }

    private void CleanHeaders(List<HarNameValuePair> headers)
    {
        for (int i = headers.Count - 1; i >= 0; i--)
        {
            var header = headers[i];
            var headerName = header.Name.ToLowerInvariant();
            
            if (_sensitiveHeaders.Any(sh => headerName.Contains(sh)))
            {
                if (_removeAuthTokens && (headerName.Contains("auth") || headerName.Contains("token")))
                {
                    headers.RemoveAt(i);
                }
                else if (_removePersonalIdentifiers && (headerName.Contains("session") || headerName.Contains("user")))
                {
                    headers.RemoveAt(i);
                }
            }
        }
    }

    private void CleanQueryString(List<HarNameValuePair> queryString)
    {
        for (int i = queryString.Count - 1; i >= 0; i--)
        {
            var param = queryString[i];
            var paramName = param.Name.ToLowerInvariant();
            
            if (_sensitiveParams.Any(sp => paramName.Contains(sp)))
            {
                if (_removeAuthTokens && (paramName.Contains("token") || paramName.Contains("auth") || paramName.Contains("key")))
                {
                    queryString.RemoveAt(i);
                }
                else if (_removePersonalIdentifiers)
                {
                    param.Value = "[REDACTED]";
                }
            }
        }
    }

    private static void RemoveHeadersByName(List<HarNameValuePair> headers, string headerName)
    {
        headers.RemoveAll(h => h.Name.Equals(headerName, StringComparison.OrdinalIgnoreCase));
    }

    private static void RemoveTrackingHeaders(List<HarNameValuePair> headers)
    {
        var trackingHeaders = new[] { "x-forwarded-for", "x-real-ip", "user-agent", "accept-language", "dnt" };
        foreach (var trackingHeader in trackingHeaders)
        {
            RemoveHeadersByName(headers, trackingHeader);
        }
    }
}
