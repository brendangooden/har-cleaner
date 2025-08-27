using HarCleaner.Filters;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Filters;

public class RequestMethodFilterTests
{
    [Fact]
    public void ShouldInclude_XhrOnly_ReturnsTrueForXhrRequests()
    {
        // Arrange
        var filter = new RequestMethodFilter(true, Array.Empty<string>(), Array.Empty<string>());
        var xhrEntry = TestDataHelper.CreateXhrEntry("https://example.com/api/data");

        // Act
        var result = filter.ShouldInclude(xhrEntry);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldInclude_XhrOnly_ReturnsFalseForRegularRequests()
    {
        // Arrange
        var filter = new RequestMethodFilter(true, Array.Empty<string>(), Array.Empty<string>());
        var regularEntry = TestDataHelper.CreateTestEntry("https://example.com/page.html");

        // Act
        var result = filter.ShouldInclude(regularEntry);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ShouldInclude_IncludeOnlyPost_ReturnsTrueOnlyForPostRequests()
    {
        // Arrange
        var filter = new RequestMethodFilter(false, new[] { "POST" }, Array.Empty<string>());
        var postEntry = TestDataHelper.CreateTestEntry("https://example.com/api", "POST");
        var getEntry = TestDataHelper.CreateTestEntry("https://example.com/page", "GET");

        // Act & Assert
        Assert.True(filter.ShouldInclude(postEntry));
        Assert.False(filter.ShouldInclude(getEntry));
    }

    [Fact]
    public void ShouldInclude_ExcludeOptions_ReturnsFalseForOptionsRequests()
    {
        // Arrange
        var filter = new RequestMethodFilter(false, Array.Empty<string>(), new[] { "OPTIONS" });
        var optionsEntry = TestDataHelper.CreateTestEntry("https://example.com/api", "OPTIONS");
        var getEntry = TestDataHelper.CreateTestEntry("https://example.com/page", "GET");

        // Act & Assert
        Assert.False(filter.ShouldInclude(optionsEntry));
        Assert.True(filter.ShouldInclude(getEntry));
    }

    [Fact]
    public void ShouldInclude_DetectsJsonResponseAsXhr()
    {
        // Arrange
        var filter = new RequestMethodFilter(true, Array.Empty<string>(), Array.Empty<string>());
        var jsonEntry = TestDataHelper.CreateTestEntry("https://example.com/api", "GET", 200, "application/json");

        // Act
        var result = filter.ShouldInclude(jsonEntry);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ShouldInclude_IncludeMultipleMethods_ReturnsTrueForAnyIncludedMethod()
    {
        // Arrange
        var filter = new RequestMethodFilter(false, new[] { "GET", "POST", "PUT" }, Array.Empty<string>());
        var getEntry = TestDataHelper.CreateTestEntry("https://example.com/page", "GET");
        var postEntry = TestDataHelper.CreateTestEntry("https://example.com/api", "POST");
        var deleteEntry = TestDataHelper.CreateTestEntry("https://example.com/api/1", "DELETE");

        // Act & Assert
        Assert.True(filter.ShouldInclude(getEntry));
        Assert.True(filter.ShouldInclude(postEntry));
        Assert.False(filter.ShouldInclude(deleteEntry));
    }
}
