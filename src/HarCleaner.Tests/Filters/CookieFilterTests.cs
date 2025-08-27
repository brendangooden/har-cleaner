using HarCleaner.Filters;
using HarCleaner.Models;

namespace HarCleaner.Tests.Filters;

public class CookieFilterTests
{
    [Fact]
    public void ShouldInclude_WithNoCookieFilters_ReturnsTrue()
    {
        // Arrange
        var filter = new CookieFilter(Array.Empty<string>(), Array.Empty<string>());
        var entry = CreateTestEntry();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(2, entry.Request.Cookies.Count);
        Assert.Equal(2, entry.Response.Cookies.Count);
    }

    [Fact]
    public void ShouldInclude_WithIncludeCookies_OnlyKeepsMatchingCookies()
    {
        // Arrange
        var filter = new CookieFilter(new[] { "session" }, Array.Empty<string>());
        var entry = CreateTestEntry();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Single(entry.Request.Cookies);
        Assert.Equal("sessionid", entry.Request.Cookies[0].Name);
        Assert.Single(entry.Response.Cookies);
        Assert.Equal("new_session", entry.Response.Cookies[0].Name);
    }

    [Fact]
    public void ShouldInclude_WithExcludeCookies_RemovesMatchingCookies()
    {
        // Arrange
        var filter = new CookieFilter(Array.Empty<string>(), new[] { "tracking" });
        var entry = CreateTestEntry();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Single(entry.Request.Cookies);
        Assert.Equal("sessionid", entry.Request.Cookies[0].Name);
        Assert.Single(entry.Response.Cookies);
        Assert.Equal("new_session", entry.Response.Cookies[0].Name);
    }

    [Fact]
    public void ShouldInclude_WithMultipleIncludePatterns_KeepsAllMatching()
    {
        // Arrange
        var filter = new CookieFilter(new[] { "session", "auth" }, Array.Empty<string>());
        var entry = CreateTestEntryWithMultipleCookies();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(3, entry.Request.Cookies.Count);
        Assert.Contains(entry.Request.Cookies, c => string.Equals(c.Name, "sessionid", StringComparison.Ordinal));
        Assert.Contains(entry.Request.Cookies, c => string.Equals(c.Name, "authtoken", StringComparison.Ordinal));
        Assert.Contains(entry.Request.Cookies, c => string.Equals(c.Name, "user_session", StringComparison.Ordinal));
    }

    [Fact]
    public void ShouldInclude_WithMultipleExcludePatterns_RemovesAllMatching()
    {
        // Arrange
        var filter = new CookieFilter(Array.Empty<string>(), new[] { "tracking", "analytics" });
        var entry = CreateTestEntryWithMultipleCookies();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(3, entry.Request.Cookies.Count);
        Assert.Contains(entry.Request.Cookies, c => string.Equals(c.Name, "sessionid", StringComparison.Ordinal));
        Assert.Contains(entry.Request.Cookies, c => string.Equals(c.Name, "authtoken", StringComparison.Ordinal));
        Assert.Contains(entry.Request.Cookies, c => string.Equals(c.Name, "user_session", StringComparison.Ordinal));
        Assert.DoesNotContain(entry.Request.Cookies, c => string.Equals(c.Name, "tracking_id", StringComparison.Ordinal));
        Assert.DoesNotContain(entry.Request.Cookies, c => string.Equals(c.Name, "analytics_data", StringComparison.Ordinal));
    }

    [Fact]
    public void ShouldInclude_WithCaseInsensitiveMatching_WorksCorrectly()
    {
        // Arrange
        var filter = new CookieFilter(new[] { "SESSION" }, Array.Empty<string>());
        var entry = CreateTestEntry();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Single(entry.Request.Cookies);
        Assert.Equal("sessionid", entry.Request.Cookies[0].Name);
    }

    [Fact]
    public void ShouldInclude_WithPatternMatching_WorksWithPartialNames()
    {
        // Arrange
        var filter = new CookieFilter(new[] { "sess" }, Array.Empty<string>());
        var entry = CreateTestEntry();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Single(entry.Request.Cookies);
        Assert.Equal("sessionid", entry.Request.Cookies[0].Name);
        Assert.Single(entry.Response.Cookies);
        Assert.Equal("new_session", entry.Response.Cookies[0].Name);
    }

    [Fact]
    public void ShouldInclude_WithIncludeAndExclude_IncludeTakesPrecedence()
    {
        // Arrange - Include takes precedence over exclude
        var filter = new CookieFilter(new[] { "session" }, new[] { "session" });
        var entry = CreateTestEntry();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Single(entry.Request.Cookies);
        Assert.Equal("sessionid", entry.Request.Cookies[0].Name);
        Assert.Single(entry.Response.Cookies);
        Assert.Equal("new_session", entry.Response.Cookies[0].Name);
    }

    [Fact]
    public void ShouldInclude_WithEmptyIncludeList_KeepsNoCookies()
    {
        // Arrange
        var filter = new CookieFilter(Array.Empty<string>(), Array.Empty<string>());
        var entry = CreateTestEntry();
        var originalRequestCount = entry.Request.Cookies.Count;
        var originalResponseCount = entry.Response.Cookies.Count;

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(originalRequestCount, entry.Request.Cookies.Count);
        Assert.Equal(originalResponseCount, entry.Response.Cookies.Count);
    }

    [Fact]
    public void ShouldInclude_WithNoMatchingIncludeCookies_RemovesAllCookies()
    {
        // Arrange
        var filter = new CookieFilter(new[] { "nonexistent" }, Array.Empty<string>());
        var entry = CreateTestEntry();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Empty(entry.Request.Cookies);
        Assert.Empty(entry.Response.Cookies);
    }

    [Fact]
    public void ShouldInclude_WithNoMatchingExcludeCookies_KeepsAllCookies()
    {
        // Arrange
        var filter = new CookieFilter(Array.Empty<string>(), new[] { "nonexistent" });
        var entry = CreateTestEntry();

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(2, entry.Request.Cookies.Count);
        Assert.Equal(2, entry.Response.Cookies.Count);
    }

    [Fact]
    public void FilterName_ReturnsCorrectName()
    {
        // Arrange
        var filter = new CookieFilter(Array.Empty<string>(), Array.Empty<string>());

        // Act & Assert
        Assert.Equal("Cookie", filter.FilterName);
    }

    private static HarEntry CreateTestEntry()
    {
        return new HarEntry
        {
            Request = new HarRequest
            {
                Cookies = new List<HarCookie>
                {
                    new() { Name = "sessionid", Value = "abc123" },
                    new() { Name = "tracking_id", Value = "xyz789" }
                }
            },
            Response = new HarResponse
            {
                Cookies = new List<HarCookie>
                {
                    new() { Name = "new_session", Value = "def456" },
                    new() { Name = "tracking_data", Value = "uvw012" }
                }
            }
        };
    }

    private static HarEntry CreateTestEntryWithMultipleCookies()
    {
        return new HarEntry
        {
            Request = new HarRequest
            {
                Cookies = new List<HarCookie>
                {
                    new() { Name = "sessionid", Value = "abc123" },
                    new() { Name = "authtoken", Value = "token123" },
                    new() { Name = "user_session", Value = "user456" },
                    new() { Name = "tracking_id", Value = "track789" },
                    new() { Name = "analytics_data", Value = "analytics012" }
                }
            },
            Response = new HarResponse
            {
                Cookies = new List<HarCookie>
                {
                    new() { Name = "new_session", Value = "session789" },
                    new() { Name = "auth_refresh", Value = "refresh123" }
                }
            }
        };
    }
}
