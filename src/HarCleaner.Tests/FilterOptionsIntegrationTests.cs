using HarCleaner.Models;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests;

public class FilterOptionsIntegrationTests
{
    [Fact]
    public void ExcludeHeadersList_ParsesCommaSeparatedValues()
    {
        // Arrange
        var options = new FilterOptions
        {
            ExcludeHeaders = "user-agent,accept-language,x-forwarded-for"
        };

        // Act
        var result = options.ExcludeHeadersList;

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Contains("user-agent", result);
        Assert.Contains("accept-language", result);
        Assert.Contains("x-forwarded-for", result);
    }

    [Fact]
    public void IncludeHeadersList_ParsesCommaSeparatedValues()
    {
        // Arrange
        var options = new FilterOptions
        {
            IncludeHeaders = "authorization,content-type"
        };

        // Act
        var result = options.IncludeHeadersList;

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Contains("authorization", result);
        Assert.Contains("content-type", result);
    }

    [Fact]
    public void ExcludeHeadersList_WithWhitespace_TrimsValues()
    {
        // Arrange
        var options = new FilterOptions
        {
            ExcludeHeaders = " user-agent , accept-language , x-forwarded-for "
        };

        // Act
        var result = options.ExcludeHeadersList;

        // Assert
        Assert.Equal(3, result.Length);
        Assert.All(result, header => Assert.DoesNotContain(" ", header, StringComparison.Ordinal));
    }

    [Fact]
    public void HeaderLists_WhenNull_ReturnsEmptyArray()
    {
        // Arrange
        var options = new FilterOptions
        {
            ExcludeHeaders = null,
            IncludeHeaders = null
        };

        // Act & Assert
        Assert.Empty(options.ExcludeHeadersList);
        Assert.Empty(options.IncludeHeadersList);
    }

    [Fact]
    public void HeaderLists_WhenEmpty_ReturnsEmptyArray()
    {
        // Arrange
        var options = new FilterOptions
        {
            ExcludeHeaders = "",
            IncludeHeaders = "   "
        };

        // Act & Assert
        Assert.Empty(options.ExcludeHeadersList);
        Assert.Empty(options.IncludeHeadersList);
    }

    [Theory]
    [InlineData("single-header", 1)]
    [InlineData("header1,header2", 2)]
    [InlineData("header1,header2,header3,header4", 4)]
    public void ExcludeHeadersList_WithVariousInputs_ParsesCorrectly(string input, int expectedCount)
    {
        // Arrange
        var options = new FilterOptions { ExcludeHeaders = input };

        // Act
        var result = options.ExcludeHeadersList;

        // Assert
        Assert.Equal(expectedCount, result.Length);
    }

    [Fact]
    public void ExcludeCookiesList_ParsesCommaSeparatedValues()
    {
        // Arrange
        var options = new FilterOptions
        {
            ExcludeCookies = "tracking,analytics,advertisement"
        };

        // Act
        var result = options.ExcludeCookiesList;

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Contains("tracking", result);
        Assert.Contains("analytics", result);
        Assert.Contains("advertisement", result);
    }

    [Fact]
    public void IncludeCookiesList_ParsesCommaSeparatedValues()
    {
        // Arrange
        var options = new FilterOptions
        {
            IncludeCookies = "session,authentication"
        };

        // Act
        var result = options.IncludeCookiesList;

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Contains("session", result);
        Assert.Contains("authentication", result);
    }

    [Fact]
    public void ExcludeCookiesList_WithWhitespace_TrimsValues()
    {
        // Arrange
        var options = new FilterOptions
        {
            ExcludeCookies = " tracking , analytics , advertisement "
        };

        // Act
        var result = options.ExcludeCookiesList;

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Contains("tracking", result);
        Assert.Contains("analytics", result);
        Assert.Contains("advertisement", result);
    }

    [Fact]
    public void IncludeCookiesList_WithNullValue_ReturnsEmpty()
    {
        // Arrange
        var options = new FilterOptions { IncludeCookies = null };

        // Act
        var result = options.IncludeCookiesList;

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void ExcludeCookiesList_WithEmptyString_ReturnsEmpty()
    {
        // Arrange
        var options = new FilterOptions { ExcludeCookies = string.Empty };

        // Act
        var result = options.ExcludeCookiesList;

        // Assert
        Assert.Empty(result);
    }

    [Theory]
    [InlineData("single-cookie", 1)]
    [InlineData("cookie1,cookie2", 2)]
    [InlineData("cookie1,cookie2,cookie3,cookie4", 4)]
    public void ExcludeCookiesList_WithVariousInputs_ParsesCorrectly(string input, int expectedCount)
    {
        // Arrange
        var options = new FilterOptions { ExcludeCookies = input };

        // Act
        var result = options.ExcludeCookiesList;

        // Assert
        Assert.Equal(expectedCount, result.Length);
    }
}
