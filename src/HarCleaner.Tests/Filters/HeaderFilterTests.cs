using HarCleaner.Filters;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Filters;

public class HeaderFilterTests
{
    [Fact]
    public void ShouldInclude_ExcludeHeaders_RemovesMatchingHeaders()
    {
        // Arrange
        var filter = new HeaderFilter(Array.Empty<string>(), new[] { "user-agent", "accept-language" });
        var entry = TestDataHelper.CreateTestEntry("https://example.com/api");

        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "User-Agent", Value = "Chrome/91.0" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Accept-Language", Value = "en-US" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Authorization", Value = "Bearer token" });

        entry.Response.Headers.Add(new Models.HarNameValuePair { Name = "Server", Value = "nginx" });
        entry.Response.Headers.Add(new Models.HarNameValuePair { Name = "Set-Cookie", Value = "session=abc" });

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Single(entry.Request.Headers);
        Assert.Equal("Authorization", entry.Request.Headers[0].Name);
        Assert.Equal(2, entry.Response.Headers.Count);
    }

    [Fact]
    public void ShouldInclude_IncludeHeaders_KeepsOnlyMatchingHeaders()
    {
        // Arrange
        var filter = new HeaderFilter(new[] { "authorization", "content-type" }, Array.Empty<string>());
        var entry = TestDataHelper.CreateTestEntry("https://example.com/api");

        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "User-Agent", Value = "Chrome/91.0" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Authorization", Value = "Bearer token" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Content-Type", Value = "application/json" });

        entry.Response.Headers.Add(new Models.HarNameValuePair { Name = "Server", Value = "nginx" });
        entry.Response.Headers.Add(new Models.HarNameValuePair { Name = "Content-Type", Value = "application/json" });

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(2, entry.Request.Headers.Count);
        Assert.Contains(entry.Request.Headers, h => string.Equals(h.Name, "Authorization", StringComparison.Ordinal));
        Assert.Contains(entry.Request.Headers, h => string.Equals(h.Name, "Content-Type", StringComparison.Ordinal));
        Assert.Single(entry.Response.Headers);
        Assert.Equal("Content-Type", entry.Response.Headers[0].Name);
    }

    [Fact]
    public void ShouldInclude_CaseInsensitiveMatching_WorksCorrectly()
    {
        // Arrange
        var filter = new HeaderFilter(Array.Empty<string>(), new[] { "USER-AGENT" });
        var entry = TestDataHelper.CreateTestEntry("https://example.com/api");

        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "user-agent", Value = "Chrome/91.0" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Authorization", Value = "Bearer token" });

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Single(entry.Request.Headers);
        Assert.Equal("Authorization", entry.Request.Headers[0].Name);
    }

    [Fact]
    public void ShouldInclude_PatternMatching_WorksCorrectly()
    {
        // Arrange
        var filter = new HeaderFilter(Array.Empty<string>(), new[] { "x-", "accept" });
        var entry = TestDataHelper.CreateTestEntry("https://example.com/api");

        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "X-Custom-Header", Value = "value1" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "X-Another-Header", Value = "value2" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Accept-Language", Value = "en-US" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Authorization", Value = "Bearer token" });

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Single(entry.Request.Headers);
        Assert.Equal("Authorization", entry.Request.Headers[0].Name);
    }

    [Fact]
    public void ShouldInclude_NoFilters_LeavesHeadersUnchanged()
    {
        // Arrange
        var filter = new HeaderFilter(Array.Empty<string>(), Array.Empty<string>());
        var entry = TestDataHelper.CreateTestEntry("https://example.com/api");

        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "User-Agent", Value = "Chrome/91.0" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Authorization", Value = "Bearer token" });

        var originalRequestHeaderCount = entry.Request.Headers.Count;
        var originalResponseHeaderCount = entry.Response.Headers.Count;

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(originalRequestHeaderCount, entry.Request.Headers.Count);
        Assert.Equal(originalResponseHeaderCount, entry.Response.Headers.Count);
    }

    [Fact]
    public void ShouldInclude_EmptyHeadersList_HandledCorrectly()
    {
        // Arrange
        var filter = new HeaderFilter(Array.Empty<string>(), new[] { "user-agent" });
        var entry = TestDataHelper.CreateTestEntry("https://example.com/api");

        // Ensure headers list is empty
        entry.Request.Headers.Clear();
        entry.Response.Headers.Clear();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Empty(entry.Request.Headers);
        Assert.Empty(entry.Response.Headers);
    }

    [Fact]
    public void ShouldInclude_IncludeAndExcludeBothSpecified_IncludeTakesPrecedence()
    {
        // Arrange - Include takes precedence over exclude
        var filter = new HeaderFilter(new[] { "authorization" }, new[] { "authorization" });
        var entry = TestDataHelper.CreateTestEntry("https://example.com/api");

        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Authorization", Value = "Bearer token" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "User-Agent", Value = "Chrome/91.0" });

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Single(entry.Request.Headers);
        Assert.Equal("Authorization", entry.Request.Headers[0].Name);
    }

    [Fact]
    public void ShouldInclude_MultiplePatterns_WorksCorrectly()
    {
        // Arrange
        var filter = new HeaderFilter(new[] { "content", "auth" }, Array.Empty<string>());
        var entry = TestDataHelper.CreateTestEntry("https://example.com/api");

        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Content-Type", Value = "application/json" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Content-Length", Value = "100" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Authorization", Value = "Bearer token" });
        entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "User-Agent", Value = "Chrome/91.0" });

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(3, entry.Request.Headers.Count);
        Assert.Contains(entry.Request.Headers, h => string.Equals(h.Name, "Content-Type", StringComparison.Ordinal));
        Assert.Contains(entry.Request.Headers, h => string.Equals(h.Name, "Content-Length", StringComparison.Ordinal));
        Assert.Contains(entry.Request.Headers, h => string.Equals(h.Name, "Authorization", StringComparison.Ordinal));
        Assert.DoesNotContain(entry.Request.Headers, h => string.Equals(h.Name, "User-Agent", StringComparison.Ordinal));
    }
}
