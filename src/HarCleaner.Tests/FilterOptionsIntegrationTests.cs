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
}
