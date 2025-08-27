using HarCleaner.Filters;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Filters;

public class UrlFilterTests
{
    [Fact]
    public void ShouldInclude_IncludeApiPattern_ReturnsTrueOnlyForApiUrls()
    {
        // Arrange
        var filter = new UrlFilter(new[] { "api" }, Array.Empty<string>());
        var apiEntry = TestDataHelper.CreateTestEntry("https://example.com/api/users");
        var pageEntry = TestDataHelper.CreateTestEntry("https://example.com/home");

        // Act & Assert
        Assert.True(filter.ShouldInclude(apiEntry));
        Assert.False(filter.ShouldInclude(pageEntry));
    }

    [Fact]
    public void ShouldInclude_ExcludeTrackingPattern_ReturnsFalseForTrackingUrls()
    {
        // Arrange
        var filter = new UrlFilter(Array.Empty<string>(), new[] { "tracking", "analytics" });
        var trackingEntry = TestDataHelper.CreateTestEntry("https://example.com/tracking/pixel");
        var analyticsEntry = TestDataHelper.CreateTestEntry("https://google-analytics.com/collect");
        var normalEntry = TestDataHelper.CreateTestEntry("https://example.com/page");

        // Act & Assert
        Assert.False(filter.ShouldInclude(trackingEntry));
        Assert.False(filter.ShouldInclude(analyticsEntry));
        Assert.True(filter.ShouldInclude(normalEntry));
    }

    [Fact]
    public void ShouldInclude_MultipleIncludePatterns_ReturnsTrueForAnyMatchingPattern()
    {
        // Arrange
        var filter = new UrlFilter(new[] { "api", "admin" }, Array.Empty<string>());
        var apiEntry = TestDataHelper.CreateTestEntry("https://example.com/api/data");
        var adminEntry = TestDataHelper.CreateTestEntry("https://example.com/admin/users");
        var publicEntry = TestDataHelper.CreateTestEntry("https://example.com/public/page");

        // Act & Assert
        Assert.True(filter.ShouldInclude(apiEntry));
        Assert.True(filter.ShouldInclude(adminEntry));
        Assert.False(filter.ShouldInclude(publicEntry));
    }

    [Fact]
    public void ShouldInclude_CaseInsensitiveMatching_WorksCorrectly()
    {
        // Arrange
        var filter = new UrlFilter(new[] { "API" }, Array.Empty<string>());
        var lowerCaseEntry = TestDataHelper.CreateTestEntry("https://example.com/api/data");
        var upperCaseEntry = TestDataHelper.CreateTestEntry("https://example.com/API/data");

        // Act & Assert
        Assert.True(filter.ShouldInclude(lowerCaseEntry));
        Assert.True(filter.ShouldInclude(upperCaseEntry));
    }

    [Fact]
    public void ShouldInclude_NoPatterns_ReturnsTrueForAllUrls()
    {
        // Arrange
        var filter = new UrlFilter(Array.Empty<string>(), Array.Empty<string>());
        var entry = TestDataHelper.CreateTestEntry("https://example.com/anything");

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldInclude_ExcludeOverridesInclude_ReturnsFalseWhenBothMatch()
    {
        // Arrange - Include "api" but exclude "test"
        var filter = new UrlFilter(new[] { "api" }, new[] { "test" });
        var testApiEntry = TestDataHelper.CreateTestEntry("https://example.com/api/test/data");

        // Act
        var result = filter.ShouldInclude(testApiEntry);

        // Assert - Exclude should take precedence
        Assert.False(result);
    }
}
