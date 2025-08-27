using HarCleaner.Models;
using HarCleaner.Services;
using Xunit;

namespace HarCleaner.Tests.Services;

public class MlIngestExporterTests
{
    [Fact]
    public void ConvertToMlIngestFormat_ShouldIncludeRequestAndResponseBodies()
    {
        // Arrange
        var harFile = CreateTestHarFile();
        var exporter = new MlIngestExporter();
        
        // Use reflection to call the private method for testing
        var method = typeof(MlIngestExporter).GetMethod("ConvertToMlIngestFormat", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // Act
        var result = (List<MlIngestEntry>)method!.Invoke(exporter, new object[] { harFile });
        
        // Assert
        Assert.Single(result);
        var entry = result.First();
        
        Assert.Equal("POST", entry.Request.Method);
        Assert.Equal("api.example.com", entry.Request.Domain);
        Assert.Equal("/users", entry.Request.Path);
        Assert.Equal(201, entry.Response.StatusCode);
        Assert.Equal("xhr", entry.RequestType);
        Assert.True(entry.Request.HasAuth);
        Assert.Contains("John Doe", entry.Request.Body);
        Assert.Contains("created", entry.Response.Body);
        Assert.Contains("sessionId=xyz789", entry.Request.Cookies);
        Assert.Contains("page=1", entry.Request.QueryParams);
    }

    [Fact]
    public void RequestType_ShouldDetectChromeDevToolsResourceTypes()
    {
        // Arrange
        var harFile = CreateHarFileWithResourceTypes();
        var exporter = new MlIngestExporter();
        
        // Use reflection to call the private method for testing
        var method = typeof(MlIngestExporter).GetMethod("ConvertToMlIngestFormat", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // Act
        var result = (List<MlIngestEntry>)method!.Invoke(exporter, new object[] { harFile });
        
        // Assert
        Assert.Equal(4, result.Count);
        
        // Document should be "document" type
        var documentEntry = result.First(e => e.ResourceType == "document");
        Assert.Equal("document", documentEntry.RequestType);
        
        // XHR should be "xhr" type
        var xhrEntry = result.First(e => e.ResourceType == "xhr");
        Assert.Equal("xhr", xhrEntry.RequestType);
        
        // Stylesheet should be "stylesheet" type
        var stylesheetEntry = result.First(e => e.ResourceType == "stylesheet");
        Assert.Equal("stylesheet", stylesheetEntry.RequestType);
        
        // Fetch should be "fetch" type
        var fetchEntry = result.First(e => e.ResourceType == "fetch");
        Assert.Equal("fetch", fetchEntry.RequestType);
    }
    
    private HarFile CreateTestHarFile()
    {
        return new HarFile
        {
            Log = new HarLog
            {
                Version = "1.2",
                Creator = new HarCreator { Name = "Test", Version = "1.0" },
                Entries = new List<HarEntry>
                {
                    new HarEntry
                    {
                        StartedDateTime = DateTime.UtcNow,
                        Time = 200,
                        Request = new HarRequest
                        {
                            Method = "POST",
                            Url = "https://api.example.com/users?page=1&limit=10",
                            Headers = new List<HarNameValuePair>
                            {
                                new HarNameValuePair { Name = "Authorization", Value = "Bearer token123" },
                                new HarNameValuePair { Name = "Content-Type", Value = "application/json" }
                            },
                            Cookies = new List<HarCookie>
                            {
                                new HarCookie { Name = "sessionId", Value = "xyz789" }
                            },
                            QueryString = new List<HarNameValuePair>
                            {
                                new HarNameValuePair { Name = "page", Value = "1" },
                                new HarNameValuePair { Name = "limit", Value = "10" }
                            },
                            PostData = new HarPostData
                            {
                                Text = "{\"name\": \"John Doe\", \"email\": \"john@example.com\"}"
                            },
                            HeadersSize = 200,
                            BodySize = 50
                        },
                        Response = new HarResponse
                        {
                            Status = 201,
                            StatusText = "Created",
                            Headers = new List<HarNameValuePair>
                            {
                                new HarNameValuePair { Name = "Content-Type", Value = "application/json" }
                            },
                            Content = new HarContent
                            {
                                Size = 100,
                                MimeType = "application/json",
                                Text = "{\"id\": 123, \"name\": \"John Doe\", \"created\": \"2025-08-27T10:00:00Z\"}"
                            },
                            HeadersSize = 150,
                            BodySize = 100
                        },
                        Cache = new HarCache(),
                        Timings = new HarTimings()
                    }
                }
            }
        };
    }

    private HarFile CreateHarFileWithResourceTypes()
    {
        return new HarFile
        {
            Log = new HarLog
            {
                Version = "1.2",
                Creator = new HarCreator { Name = "Chrome DevTools", Version = "120.0" },
                Entries = new List<HarEntry>
                {
                    // Document request
                    new HarEntry
                    {
                        StartedDateTime = DateTime.UtcNow,
                        Time = 200,
                        ResourceType = "document",
                        Request = new HarRequest
                        {
                            Method = "GET",
                            Url = "https://example.com/index.html",
                            Headers = new List<HarNameValuePair>
                            {
                                new HarNameValuePair { Name = "Accept", Value = "text/html" }
                            },
                            HeadersSize = 200,
                            BodySize = 0
                        },
                        Response = new HarResponse
                        {
                            Status = 200,
                            StatusText = "OK",
                            Content = new HarContent
                            {
                                Size = 5000,
                                MimeType = "text/html",
                                Text = "<!DOCTYPE html><html>...</html>"
                            },
                            HeadersSize = 100,
                            BodySize = 5000
                        },
                        Cache = new HarCache(),
                        Timings = new HarTimings()
                    },
                    // XHR request
                    new HarEntry
                    {
                        StartedDateTime = DateTime.UtcNow,
                        Time = 150,
                        ResourceType = "xhr",
                        Request = new HarRequest
                        {
                            Method = "POST",
                            Url = "https://api.example.com/users",
                            Headers = new List<HarNameValuePair>
                            {
                                new HarNameValuePair { Name = "Content-Type", Value = "application/json" },
                                new HarNameValuePair { Name = "X-Requested-With", Value = "XMLHttpRequest" }
                            },
                            PostData = new HarPostData
                            {
                                Text = "{\"name\": \"John Doe\"}"
                            },
                            HeadersSize = 250,
                            BodySize = 20
                        },
                        Response = new HarResponse
                        {
                            Status = 201,
                            StatusText = "Created",
                            Content = new HarContent
                            {
                                Size = 50,
                                MimeType = "application/json",
                                Text = "{\"id\": 123}"
                            },
                            HeadersSize = 180,
                            BodySize = 50
                        },
                        Cache = new HarCache(),
                        Timings = new HarTimings()
                    },
                    // Stylesheet request
                    new HarEntry
                    {
                        StartedDateTime = DateTime.UtcNow,
                        Time = 50,
                        ResourceType = "stylesheet",
                        Request = new HarRequest
                        {
                            Method = "GET",
                            Url = "https://example.com/style.css",
                            Headers = new List<HarNameValuePair>
                            {
                                new HarNameValuePair { Name = "Accept", Value = "text/css" }
                            },
                            HeadersSize = 180,
                            BodySize = 0
                        },
                        Response = new HarResponse
                        {
                            Status = 200,
                            StatusText = "OK",
                            Content = new HarContent
                            {
                                Size = 2000,
                                MimeType = "text/css",
                                Text = "body { font-family: Arial; }"
                            },
                            HeadersSize = 120,
                            BodySize = 2000
                        },
                        Cache = new HarCache(),
                        Timings = new HarTimings()
                    },
                    // Fetch request
                    new HarEntry
                    {
                        StartedDateTime = DateTime.UtcNow,
                        Time = 100,
                        ResourceType = "fetch",
                        Request = new HarRequest
                        {
                            Method = "GET",
                            Url = "https://api.example.com/data",
                            Headers = new List<HarNameValuePair>
                            {
                                new HarNameValuePair { Name = "Accept", Value = "application/json" }
                            },
                            HeadersSize = 180,
                            BodySize = 0
                        },
                        Response = new HarResponse
                        {
                            Status = 200,
                            StatusText = "OK",
                            Content = new HarContent
                            {
                                Size = 500,
                                MimeType = "application/json",
                                Text = "[{\"id\": 1, \"name\": \"Item 1\"}]"
                            },
                            HeadersSize = 150,
                            BodySize = 500
                        },
                        Cache = new HarCache(),
                        Timings = new HarTimings()
                    }
                }
            }
        };
    }
}
