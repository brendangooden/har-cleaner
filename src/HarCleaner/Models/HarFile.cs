using System.Text.Json.Serialization;

namespace HarCleaner.Models;

public class HarFile
{
    [JsonPropertyName("log")]
    public HarLog Log { get; set; } = new();
}

public class HarLog
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("creator")]
    public HarCreator Creator { get; set; } = new();

    [JsonPropertyName("browser")]
    public HarBrowser? Browser { get; set; }

    [JsonPropertyName("pages")]
    public List<HarPage> Pages { get; set; } = new();

    [JsonPropertyName("entries")]
    public List<HarEntry> Entries { get; set; } = new();

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

public class HarCreator
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

public class HarBrowser
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

public class HarPage
{
    [JsonPropertyName("startedDateTime")]
    public DateTime StartedDateTime { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("pageTimings")]
    public HarPageTimings PageTimings { get; set; } = new();

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

public class HarPageTimings
{
    [JsonPropertyName("onContentLoad")]
    public double? OnContentLoad { get; set; }

    [JsonPropertyName("onLoad")]
    public double? OnLoad { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
