using CommandLine;

namespace HarCleaner.Models;

public class FilterOptions
{
    [Option('i', "input", Required = true, HelpText = "Input HAR file path")]
    public string InputFile { get; set; } = string.Empty;

    [Option('o', "output", Required = true, HelpText = "Output HAR file path")]
    public string OutputFile { get; set; } = string.Empty;

    [Option("exclude-types", HelpText = "Comma-separated list of file types to exclude (e.g., js,css,png)")]
    public string? ExcludeTypes { get; set; }

    [Option("include-types", HelpText = "Comma-separated list of file types to include only")]
    public string? IncludeTypes { get; set; }

    [Option("xhr-only", HelpText = "Include only XHR/AJAX requests")]
    public bool XhrOnly { get; set; }

    [Option("include-url", HelpText = "Comma-separated list of URL patterns that must be present")]
    public string? IncludeUrlPatterns { get; set; }

    [Option("exclude-url", HelpText = "Comma-separated list of URL patterns to exclude")]
    public string? ExcludeUrlPatterns { get; set; }

    [Option("include-methods", HelpText = "Comma-separated list of HTTP methods to include (e.g., GET,POST)")]
    public string? IncludeMethods { get; set; }

    [Option("exclude-methods", HelpText = "Comma-separated list of HTTP methods to exclude")]
    public string? ExcludeMethods { get; set; }

    [Option("min-size", HelpText = "Minimum response size in bytes")]
    public int? MinSize { get; set; }

    [Option("max-size", HelpText = "Maximum response size in bytes")]
    public int? MaxSize { get; set; }

    [Option("include-status", HelpText = "Comma-separated list of status codes to include (e.g., 200,404)")]
    public string? IncludeStatusCodes { get; set; }

    [Option("exclude-status", HelpText = "Comma-separated list of status codes to exclude")]
    public string? ExcludeStatusCodes { get; set; }

    [Option('v', "verbose", HelpText = "Enable verbose output")]
    public bool Verbose { get; set; }

    [Option("dry-run", HelpText = "Preview changes without saving")]
    public bool DryRun { get; set; }

    [Option("output-type", Default = "har", HelpText = "Output format: 'har' (default) or 'ml-ingest' (simplified for ML)")]
    public string OutputType { get; set; } = "har";

    // Privacy and content filtering options
    [Option("remove-cookies", HelpText = "Remove all cookies from requests and responses")]
    public bool RemoveCookies { get; set; }

    [Option("remove-auth", HelpText = "Remove authentication tokens and headers")]
    public bool RemoveAuthTokens { get; set; }

    [Option("remove-personal", HelpText = "Remove personal identifiers (sessions, user info)")]
    public bool RemovePersonalIdentifiers { get; set; }

    [Option("remove-tracking", HelpText = "Remove tracking headers (user-agent, accept-language, etc.)")]
    public bool RemoveTrackingHeaders { get; set; }

    [Option("remove-response-content", HelpText = "Remove response body content")]
    public bool RemoveResponseContent { get; set; }

    [Option("remove-request-content", HelpText = "Remove request body content (POST data)")]
    public bool RemoveRequestContent { get; set; }

    [Option("remove-base64", HelpText = "Remove base64 encoded content")]
    public bool RemoveBase64Content { get; set; }

    [Option("max-content-size", HelpText = "Maximum content size in bytes (larger content will be removed)")]
    public long? MaxContentSize { get; set; }

    [Option("exclude-content-types", HelpText = "Comma-separated list of content types to remove content from")]
    public string? ExcludeContentTypes { get; set; }

    [Option("remove-chrome-data", HelpText = "Remove Chrome DevTools specific fields (_connectionId, _initiator, etc.)")]
    public bool RemoveChromeData { get; set; }

    // Header filtering options
    [Option("exclude-headers", HelpText = "Comma-separated list of header names or patterns to exclude from requests and responses")]
    public string? ExcludeHeaders { get; set; }

    [Option("include-headers", HelpText = "Comma-separated list of header names or patterns to include in requests and responses (others will be removed)")]
    public string? IncludeHeaders { get; set; }

    // Helper properties to parse comma-separated values
    public string[] ExcludeTypesList => ParseCommaSeparated(ExcludeTypes);
    public string[] IncludeTypesList => ParseCommaSeparated(IncludeTypes);
    public string[] IncludeUrlPatternsList => ParseCommaSeparated(IncludeUrlPatterns);
    public string[] ExcludeUrlPatternsList => ParseCommaSeparated(ExcludeUrlPatterns);
    public string[] IncludeMethodsList => ParseCommaSeparated(IncludeMethods);
    public string[] ExcludeMethodsList => ParseCommaSeparated(ExcludeMethods);
    public int[] IncludeStatusCodesList => ParseCommaSeparatedInt(IncludeStatusCodes);
    public int[] ExcludeStatusCodesList => ParseCommaSeparatedInt(ExcludeStatusCodes);
    public string[] ExcludeContentTypesList => ParseCommaSeparated(ExcludeContentTypes);

    public string[] ExcludeHeadersList => ParseCommaSeparated(ExcludeHeaders);
    public string[] IncludeHeadersList => ParseCommaSeparated(IncludeHeaders);

    private static string[] ParseCommaSeparated(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<string>();
        }

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => s.Trim())
                   .ToArray();
    }

    private static int[] ParseCommaSeparatedInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<int>();
        }

        return value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(s => s.Trim())
                   .Where(s => int.TryParse(s, out _))
                   .Select(int.Parse)
                   .ToArray();
    }
}
