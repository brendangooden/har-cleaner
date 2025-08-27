using HarCleaner.Filters;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Filters;

public class StatusCodeFilterTests
{
    [Fact]
    public void ShouldInclude_IncludeSuccessStatusCodes_ReturnsTrueOnlyForSuccessfulResponses()
    {
        // Arrange
        var filter = new StatusCodeFilter(new[] { 200, 201, 202 }, Array.Empty<int>());
        var okEntry = TestDataHelper.CreateTestEntry("https://example.com/api", "GET", 200);
        var createdEntry = TestDataHelper.CreateTestEntry("https://example.com/api", "POST", 201);
        var notFoundEntry = TestDataHelper.CreateTestEntry("https://example.com/missing", "GET", 404);

        // Act & Assert
        Assert.True(filter.ShouldInclude(okEntry));
        Assert.True(filter.ShouldInclude(createdEntry));
        Assert.False(filter.ShouldInclude(notFoundEntry));
    }

    [Fact]
    public void ShouldInclude_ExcludeErrorStatusCodes_ReturnsFalseForErrorResponses()
    {
        // Arrange
        var filter = new StatusCodeFilter(Array.Empty<int>(), new[] { 404, 500, 502 });
        var okEntry = TestDataHelper.CreateTestEntry("https://example.com/page", "GET", 200);
        var notFoundEntry = TestDataHelper.CreateTestEntry("https://example.com/missing", "GET", 404);
        var serverErrorEntry = TestDataHelper.CreateTestEntry("https://example.com/error", "GET", 500);

        // Act & Assert
        Assert.True(filter.ShouldInclude(okEntry));
        Assert.False(filter.ShouldInclude(notFoundEntry));
        Assert.False(filter.ShouldInclude(serverErrorEntry));
    }

    [Fact]
    public void ShouldInclude_NoStatusCodeFilters_ReturnsTrueForAllResponses()
    {
        // Arrange
        var filter = new StatusCodeFilter(Array.Empty<int>(), Array.Empty<int>());
        var okEntry = TestDataHelper.CreateTestEntry("https://example.com/page", "GET", 200);
        var errorEntry = TestDataHelper.CreateTestEntry("https://example.com/error", "GET", 500);

        // Act & Assert
        Assert.True(filter.ShouldInclude(okEntry));
        Assert.True(filter.ShouldInclude(errorEntry));
    }

    [Fact]
    public void ShouldInclude_IncludeRedirectStatusCodes_ReturnsTrueForRedirects()
    {
        // Arrange
        var filter = new StatusCodeFilter(new[] { 301, 302, 307 }, Array.Empty<int>());
        var redirectEntry = TestDataHelper.CreateTestEntry("https://example.com/redirect", "GET", 302);
        var okEntry = TestDataHelper.CreateTestEntry("https://example.com/page", "GET", 200);

        // Act & Assert
        Assert.True(filter.ShouldInclude(redirectEntry));
        Assert.False(filter.ShouldInclude(okEntry));
    }
}
