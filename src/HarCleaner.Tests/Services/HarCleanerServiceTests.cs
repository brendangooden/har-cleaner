using HarCleaner.Services;
using HarCleaner.Filters;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Services;

public class HarCleanerServiceTests
{
	[Fact]
	public void Clean_NoFilters_ReturnsAllEntries()
	{
		// Arrange
		var service = new HarCleanerService();
		var harFile = TestDataHelper.CreateTestHarFile();
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/page1"));
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/page2"));

		// Act
		var result = service.Clean(harFile);

		// Assert
		Assert.Equal(2, result.OriginalCount);
		Assert.Equal(2, result.FilteredCount);
		Assert.Equal(0, result.RemovedCount);
		Assert.Equal(0, result.RemovalPercentage);
	}

	[Fact]
	public void Clean_WithUrlFilter_FiltersCorrectly()
	{
		// Arrange
		var service = new HarCleanerService();
		service.AddFilter(new UrlFilter(new[] { "api" }, Array.Empty<string>()));

		var harFile = TestDataHelper.CreateTestHarFile();
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/api/users"));
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/page"));
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/api/data"));

		// Act
		var result = service.Clean(harFile);

		// Assert
		Assert.Equal(3, result.OriginalCount);
		Assert.Equal(2, result.FilteredCount); // Only API URLs should remain
		Assert.Equal(1, result.RemovedCount);
		Assert.Equal(33.3, result.RemovalPercentage, 1); // Approximately 33.3%
	}

	[Fact]
	public void Clean_WithHeaderFilter_ModifiesHeadersCorrectly()
	{
		// Arrange
		var service = new HarCleanerService();
		service.AddFilter(new HeaderFilter(Array.Empty<string>(), new[] { "user-agent" }));

		var harFile = TestDataHelper.CreateTestHarFile();
		var entry = TestDataHelper.CreateTestEntry("https://example.com/api");
		entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "User-Agent", Value = "Chrome/91.0" });
		entry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Authorization", Value = "Bearer token" });
		harFile.Log.Entries.Add(entry);

		// Act
		var result = service.Clean(harFile);

		// Assert
		Assert.Equal(1, result.OriginalCount);
		Assert.Equal(1, result.FilteredCount); // Entry should remain
		Assert.Equal(0, result.RemovedCount);

		// Verify headers were filtered
		var cleanedEntry = result.CleanedHarFile.Log.Entries[0];
		Assert.Single(cleanedEntry.Request.Headers);
		Assert.Equal("Authorization", cleanedEntry.Request.Headers[0].Name);
	}

	[Fact]
	public void Clean_WithHeaderAndUrlFilters_AppliesAllFilters()
	{
		// Arrange
		var service = new HarCleanerService();
		service.AddFilter(new UrlFilter(new[] { "api" }, Array.Empty<string>()));
		service.AddFilter(new HeaderFilter(new[] { "authorization" }, Array.Empty<string>()));

		var harFile = TestDataHelper.CreateTestHarFile();

		var apiEntry = TestDataHelper.CreateTestEntry("https://example.com/api/users");
		apiEntry.Request.Headers.Add(new Models.HarNameValuePair { Name = "User-Agent", Value = "Chrome/91.0" });
		apiEntry.Request.Headers.Add(new Models.HarNameValuePair { Name = "Authorization", Value = "Bearer token" });
		harFile.Log.Entries.Add(apiEntry);

		var pageEntry = TestDataHelper.CreateTestEntry("https://example.com/page");
		harFile.Log.Entries.Add(pageEntry);

		// Act
		var result = service.Clean(harFile);

		// Assert
		Assert.Equal(2, result.OriginalCount);
		Assert.Equal(1, result.FilteredCount); // Only API entry should remain
		Assert.Equal(1, result.RemovedCount);

		// Verify headers were filtered on remaining entry
		var cleanedEntry = result.CleanedHarFile.Log.Entries[0];
		Assert.Single(cleanedEntry.Request.Headers);
		Assert.Equal("Authorization", cleanedEntry.Request.Headers[0].Name);
	}

	[Fact]
	public void Clean_WithMultipleFilters_AppliesAllFilters()
	{
		// Arrange
		var service = new HarCleanerService();
		service.AddFilter(new UrlFilter(new[] { "api" }, Array.Empty<string>()));
		service.AddFilter(new StatusCodeFilter(new[] { 200 }, Array.Empty<int>()));

		var harFile = TestDataHelper.CreateTestHarFile();
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/api/users", "GET", 200));
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/api/error", "GET", 404));
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/page", "GET", 200));

		// Act
		var result = service.Clean(harFile);

		// Assert
		Assert.Equal(3, result.OriginalCount);
		Assert.Equal(1, result.FilteredCount); // Only API URL with 200 status should remain
		Assert.Equal(2, result.RemovedCount);
	}

	[Fact]
	public void Clean_VerboseMode_ReturnsExcludedEntries()
	{
		// Arrange
		var service = new HarCleanerService();
		service.AddFilter(new StatusCodeFilter(new[] { 200 }, Array.Empty<int>()));

		var harFile = TestDataHelper.CreateTestHarFile();
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/success", "GET", 200));
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/error", "GET", 404));

		// Act
		var result = service.Clean(harFile, verbose: true);

		// Assert
		Assert.Equal(2, result.OriginalCount);
		Assert.Equal(1, result.FilteredCount);
		Assert.Single(result.ExcludedEntries);

		var excludedEntry = result.ExcludedEntries[0];
		Assert.Equal("https://example.com/error", excludedEntry.Url);
		Assert.Equal("GET", excludedEntry.Method);
		Assert.Equal(404, excludedEntry.Status);
		Assert.Contains("StatusCode", excludedEntry.Reasons);
	}

	[Fact]
	public void Clean_PreservesHarFileStructure()
	{
		// Arrange
		var service = new HarCleanerService();
		var harFile = TestDataHelper.CreateTestHarFile();
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/page"));

		var originalVersion = harFile.Log.Version;
		var originalCreator = harFile.Log.Creator.Name;
		var originalPageCount = harFile.Log.Pages.Count;

		// Act
		var result = service.Clean(harFile);

		// Assert
		Assert.Equal(originalVersion, result.CleanedHarFile.Log.Version);
		Assert.Equal(originalCreator, result.CleanedHarFile.Log.Creator.Name);
		Assert.Equal(originalPageCount, result.CleanedHarFile.Log.Pages.Count);
	}

	[Fact]
	public void Clean_FilterThatExcludesAllEntries_ReturnsEmptyEntries()
	{
		// Arrange
		var service = new HarCleanerService();
		service.AddFilter(new StatusCodeFilter(new[] { 999 }, Array.Empty<int>())); // No entries will have 999 status

		var harFile = TestDataHelper.CreateTestHarFile();
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/page1", "GET", 200));
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/page2", "GET", 404));

		// Act
		var result = service.Clean(harFile);

		// Assert
		Assert.Equal(2, result.OriginalCount);
		Assert.Equal(0, result.FilteredCount);
		Assert.Equal(2, result.RemovedCount);
		Assert.Equal(100, result.RemovalPercentage);
		Assert.Empty(result.CleanedHarFile.Log.Entries);
	}

	[Fact]
	public void Clean_PrivacyFilter_ModifiesEntriesInPlace()
	{
		// Arrange
		var service = new HarCleanerService();
		service.AddFilter(new PrivacyFilter(removeCookies: true));

		var harFile = TestDataHelper.CreateTestHarFile();
		var entryWithCookies = TestDataHelper.CreateEntryWithCookies("https://example.com/page");
		harFile.Log.Entries.Add(entryWithCookies);

		// Act
		var result = service.Clean(harFile);

		// Assert
		Assert.Equal(1, result.OriginalCount);
		Assert.Equal(1, result.FilteredCount); // Entry is included but modified
		Assert.Equal(0, result.RemovedCount);

		// Verify the entry was modified
		var cleanedEntry = result.CleanedHarFile.Log.Entries[0];
		Assert.Empty(cleanedEntry.Request.Cookies); // Cookies should be removed
	}
}
