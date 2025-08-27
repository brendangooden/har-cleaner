using HarCleaner.Filters;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Filters;

public class SizeFilterTests
{
	[Fact]
	public void ShouldInclude_MinSizeFilter_ReturnsTrueOnlyForLargeResponses()
	{
		// Arrange
		var filter = new SizeFilter(1000, null);
		var largeEntry = TestDataHelper.CreateTestEntry("https://example.com/large", "GET", 200, "text/html", 2000);
		var smallEntry = TestDataHelper.CreateTestEntry("https://example.com/small", "GET", 200, "text/html", 500);

		// Act & Assert
		Assert.True(filter.ShouldInclude(largeEntry));
		Assert.False(filter.ShouldInclude(smallEntry));
	}

	[Fact]
	public void ShouldInclude_MaxSizeFilter_ReturnsTrueOnlyForSmallResponses()
	{
		// Arrange
		var filter = new SizeFilter(null, 1000);
		var largeEntry = TestDataHelper.CreateTestEntry("https://example.com/large", "GET", 200, "text/html", 2000);
		var smallEntry = TestDataHelper.CreateTestEntry("https://example.com/small", "GET", 200, "text/html", 500);

		// Act & Assert
		Assert.False(filter.ShouldInclude(largeEntry));
		Assert.True(filter.ShouldInclude(smallEntry));
	}

	[Fact]
	public void ShouldInclude_MinAndMaxSizeFilter_ReturnsTrueOnlyForMediumResponses()
	{
		// Arrange
		var filter = new SizeFilter(500, 1500);
		var tooSmallEntry = TestDataHelper.CreateTestEntry("https://example.com/tiny", "GET", 200, "text/html", 200);
		var mediumEntry = TestDataHelper.CreateTestEntry("https://example.com/medium", "GET", 200, "text/html", 1000);
		var tooLargeEntry = TestDataHelper.CreateTestEntry("https://example.com/huge", "GET", 200, "text/html", 2000);

		// Act & Assert
		Assert.False(filter.ShouldInclude(tooSmallEntry));
		Assert.True(filter.ShouldInclude(mediumEntry));
		Assert.False(filter.ShouldInclude(tooLargeEntry));
	}

	[Fact]
	public void ShouldInclude_NoSizeFilters_ReturnsTrueForAllResponses()
	{
		// Arrange
		var filter = new SizeFilter(null, null);
		var entry = TestDataHelper.CreateTestEntry("https://example.com/page", "GET", 200, "text/html", 1000);

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void ShouldInclude_BoundaryValues_WorksCorrectly()
	{
		// Arrange
		var filter = new SizeFilter(1000, 2000);
		var exactMinEntry = TestDataHelper.CreateTestEntry("https://example.com/min", "GET", 200, "text/html", 1000);
		var exactMaxEntry = TestDataHelper.CreateTestEntry("https://example.com/max", "GET", 200, "text/html", 2000);

		// Act & Assert
		Assert.True(filter.ShouldInclude(exactMinEntry));
		Assert.True(filter.ShouldInclude(exactMaxEntry));
	}
}
