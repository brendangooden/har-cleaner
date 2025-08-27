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
        
        Assert.Equal("POST", entry.Method);
        Assert.Equal("api.example.com", entry.Domain);
        Assert.Equal("/users", entry.Path);
        Assert.Equal(201, entry.StatusCode);
        Assert.True(entry.IsApiCall);
        Assert.True(entry.HasAuth);
        Assert.Contains("John Doe", entry.RequestBody);
        Assert.Contains("created", entry.ResponseBody);
        Assert.Contains("sessionId=xyz789", entry.Cookies);
        Assert.Contains("page=1", entry.QueryParams);
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
}
