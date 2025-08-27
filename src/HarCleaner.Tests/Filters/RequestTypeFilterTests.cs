using HarCleaner.Filters;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Filters;

public class RequestTypeFilterTests
{
	[Fact]
	public void ShouldInclude_ExcludeJavaScript_ReturnsFalseForJsFiles()
	{
		// Arrange
		var filter = new RequestTypeFilter(Array.Empty<string>(), new[] { "js" });
		var entry = TestDataHelper.CreateStaticAssetEntry("https://example.com/script.js", "js");

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void ShouldInclude_ExcludeCss_ReturnsFalseForCssFiles()
	{
		// Arrange
		var filter = new RequestTypeFilter(Array.Empty<string>(), new[] { "css" });
		var entry = TestDataHelper.CreateStaticAssetEntry("https://example.com/style.css", "css");

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.False(result);
	}

	[Fact]
	public void ShouldInclude_ExcludeMultipleTypes_ReturnsFalseForAllExcludedTypes()
	{
		// Arrange
		var filter = new RequestTypeFilter(Array.Empty<string>(), new[] { "js", "css", "png" });
		var jsEntry = TestDataHelper.CreateStaticAssetEntry("https://example.com/script.js", "js");
		var cssEntry = TestDataHelper.CreateStaticAssetEntry("https://example.com/style.css", "css");
		var pngEntry = TestDataHelper.CreateStaticAssetEntry("https://example.com/image.png", "png");

		// Act & Assert
		Assert.False(filter.ShouldInclude(jsEntry));
		Assert.False(filter.ShouldInclude(cssEntry));
		Assert.False(filter.ShouldInclude(pngEntry));
	}

	[Fact]
	public void ShouldInclude_IncludeOnlyJson_ReturnsTrueOnlyForJsonFiles()
	{
		// Arrange
		var filter = new RequestTypeFilter(new[] { "json" }, Array.Empty<string>());
		var jsonEntry = TestDataHelper.CreateTestEntry("https://example.com/api/data", "GET", 200, "application/json");
		var htmlEntry = TestDataHelper.CreateTestEntry("https://example.com/page", "GET", 200, "text/html");

		// Act & Assert
		Assert.True(filter.ShouldInclude(jsonEntry));
		Assert.False(filter.ShouldInclude(htmlEntry));
	}

	[Fact]
	public void ShouldInclude_NoFilters_ReturnsTrueForAllEntries()
	{
		// Arrange
		var filter = new RequestTypeFilter(Array.Empty<string>(), Array.Empty<string>());
		var entry = TestDataHelper.CreateTestEntry("https://example.com/anything");

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
	}

	[Fact]
	public void ShouldInclude_ExcludeImages_ReturnsFalseForImageMimeTypes()
	{
		// Arrange
		var filter = new RequestTypeFilter(Array.Empty<string>(), new[] { "png", "jpg" });
		var pngEntry = TestDataHelper.CreateTestEntry("https://example.com/image", "GET", 200, "image/png");
		var jpegEntry = TestDataHelper.CreateTestEntry("https://example.com/photo", "GET", 200, "image/jpeg");

		// Act & Assert
		Assert.False(filter.ShouldInclude(pngEntry));
		Assert.False(filter.ShouldInclude(jpegEntry));
	}
}
