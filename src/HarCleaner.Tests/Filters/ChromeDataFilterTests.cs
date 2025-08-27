using HarCleaner.Filters;
using HarCleaner.Models;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Filters;

public class ChromeDataFilterTests
{
	[Fact]
	public void ShouldInclude_RemoveConnectionIds_ClearsConnectionIdField()
	{
		// Arrange
		var filter = new ChromeDataFilter(removeConnectionIds: true);
		var entry = CreateChromeEntry();
		entry.ConnectionId = "chrome-connection-123";

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result); // Filter doesn't exclude entries
		Assert.Null(entry.ConnectionId);
	}

	[Fact]
	public void ShouldInclude_RemoveInitiatorData_ClearsInitiatorField()
	{
		// Arrange
		var filter = new ChromeDataFilter(removeInitiatorData: true);
		var entry = CreateChromeEntry();
		entry.Initiator = new { type = "script", url = "https://example.com/script.js" };

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Null(entry.Initiator);
	}

	[Fact]
	public void ShouldInclude_RemovePriorityData_ClearsPriorityField()
	{
		// Arrange
		var filter = new ChromeDataFilter(removePriorityData: true);
		var entry = CreateChromeEntry();
		entry.Priority = "High";

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Null(entry.Priority);
	}

	[Fact]
	public void ShouldInclude_RemoveResourceTypeData_ClearsResourceTypeField()
	{
		// Arrange
		var filter = new ChromeDataFilter(removeResourceTypeData: true);
		var entry = CreateChromeEntry();
		entry.ResourceType = "xhr";

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Null(entry.ResourceType);
	}

	[Fact]
	public void ShouldInclude_RemoveInternalTimings_ClearsTimingComments()
	{
		// Arrange
		var filter = new ChromeDataFilter(removeInternalTimings: true);
		var entry = CreateChromeEntry();
		entry.Timings.Comment = "Chrome DevTools queueing time: 10ms";
		entry.Comment = "Chrome DevTools specific timing data";

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Null(entry.Timings.Comment);
		Assert.Null(entry.Comment);
	}

	[Fact]
	public void ShouldInclude_RemoveInternalTimings_PreservesNonChromeComments()
	{
		// Arrange
		var filter = new ChromeDataFilter(removeInternalTimings: true);
		var entry = CreateChromeEntry();
		entry.Timings.Comment = "Standard HAR timing information";
		entry.Comment = "User-defined comment";

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Equal("Standard HAR timing information", entry.Timings.Comment);
		Assert.Equal("User-defined comment", entry.Comment);
	}

	[Fact]
	public void ShouldInclude_RemoveTransferSizes_RemovesChromeSpecificHeaders()
	{
		// Arrange
		var filter = new ChromeDataFilter(removeTransferSizes: true);
		var entry = CreateChromeEntry();
		
		// Add Chrome-specific headers
		entry.Response.Headers.Add(new HarNameValuePair { Name = "x-devtools-response-time", Value = "123" });
		entry.Response.Headers.Add(new HarNameValuePair { Name = "x-chrome-network-id", Value = "456" });
		entry.Response.Headers.Add(new HarNameValuePair { Name = "x-transfer-size", Value = "789" });
		entry.Response.Headers.Add(new HarNameValuePair { Name = "content-type", Value = "application/json" });
		
		entry.Request.Headers.Add(new HarNameValuePair { Name = "x-devtools-request-id", Value = "abc" });
		entry.Request.Headers.Add(new HarNameValuePair { Name = "accept", Value = "application/json" });

		var originalResponseHeaderCount = entry.Response.Headers.Count;
		var originalRequestHeaderCount = entry.Request.Headers.Count;

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		
		// Chrome-specific headers should be removed
		Assert.DoesNotContain(entry.Response.Headers, h => h.Name.StartsWith("x-devtools-", StringComparison.Ordinal));
		Assert.DoesNotContain(entry.Response.Headers, h => h.Name.StartsWith("x-chrome-", StringComparison.Ordinal));
		Assert.DoesNotContain(entry.Response.Headers, h => h.Name.StartsWith("x-transfer-size", StringComparison.Ordinal));
		Assert.DoesNotContain(entry.Request.Headers, h => h.Name.StartsWith("x-devtools-", StringComparison.Ordinal));
		
		// Standard headers should remain
		Assert.Contains(entry.Response.Headers, h => string.Equals(h.Name, "content-type", StringComparison.Ordinal));
		Assert.Contains(entry.Request.Headers, h => string.Equals(h.Name, "accept", StringComparison.Ordinal));
		
		// Verify some headers were actually removed
		Assert.True(entry.Response.Headers.Count < originalResponseHeaderCount);
		Assert.True(entry.Request.Headers.Count < originalRequestHeaderCount);
	}

	[Fact]
	public void ShouldInclude_AllChromeDataOptionsEnabled_CleansAllChromeSpecificData()
	{
		// Arrange
		var filter = new ChromeDataFilter(
			removeConnectionIds: true,
			removeInitiatorData: true,
			removePriorityData: true,
			removeResourceTypeData: true,
			removeInternalTimings: true,
			removeTransferSizes: true);

		var entry = CreateChromeEntry();
		
		// Set all Chrome-specific data
		entry.ConnectionId = "chrome-connection-123";
		entry.Initiator = new { type = "script" };
		entry.Priority = "High";
		entry.ResourceType = "xhr";
		entry.Timings.Comment = "Chrome queueing data";
		entry.Comment = "Chrome DevTools data";
		entry.Response.Headers.Add(new HarNameValuePair { Name = "x-devtools-timing", Value = "123" });

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Null(entry.ConnectionId);
		Assert.Null(entry.Initiator);
		Assert.Null(entry.Priority);
		Assert.Null(entry.ResourceType);
		Assert.Null(entry.Timings.Comment);
		Assert.Null(entry.Comment);
		Assert.DoesNotContain(entry.Response.Headers, h => h.Name.StartsWith("x-devtools-", StringComparison.Ordinal));
	}

	[Fact]
	public void ShouldInclude_NoChromeDataOptionsEnabled_LeavesDataUnchanged()
	{
		// Arrange
		var filter = new ChromeDataFilter();
		var entry = CreateChromeEntry();
		
		// Set Chrome-specific data
		entry.ConnectionId = "chrome-connection-123";
		entry.Initiator = new { type = "script" };
		entry.Priority = "High";
		entry.ResourceType = "xhr";
		entry.Timings.Comment = "Chrome queueing data";
		entry.Comment = "Chrome DevTools data";
		entry.Response.Headers.Add(new HarNameValuePair { Name = "x-devtools-timing", Value = "123" });

		var originalHeaderCount = entry.Response.Headers.Count;

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Equal("chrome-connection-123", entry.ConnectionId);
		Assert.NotNull(entry.Initiator);
		Assert.Equal("High", entry.Priority);
		Assert.Equal("xhr", entry.ResourceType);
		Assert.Equal("Chrome queueing data", entry.Timings.Comment);
		Assert.Equal("Chrome DevTools data", entry.Comment);
		Assert.Equal(originalHeaderCount, entry.Response.Headers.Count);
	}

	[Fact]
	public void ShouldInclude_RemoveConnectionIdsOnly_OnlyClearsConnectionIds()
	{
		// Arrange
		var filter = new ChromeDataFilter(removeConnectionIds: true);
		var entry = CreateChromeEntry();
		
		// Set all Chrome-specific data
		entry.ConnectionId = "chrome-connection-123";
		entry.Initiator = new { type = "script" };
		entry.Priority = "High";
		entry.ResourceType = "xhr";

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Null(entry.ConnectionId); // Should be cleared
		Assert.NotNull(entry.Initiator); // Should remain
		Assert.Equal("High", entry.Priority); // Should remain
		Assert.Equal("xhr", entry.ResourceType); // Should remain
	}

	[Fact]
	public void FilterName_ReturnsCorrectName()
	{
		// Arrange
		var filter = new ChromeDataFilter();

		// Act & Assert
		Assert.Equal("ChromeData", filter.FilterName);
	}

	private static HarEntry CreateChromeEntry()
	{
		return new HarEntry
		{
			StartedDateTime = DateTime.UtcNow,
			Time = 100,
			Request = new HarRequest
			{
				Method = "GET",
				Url = "https://example.com/api/data",
				Headers = new List<HarNameValuePair>(),
				Cookies = new List<HarCookie>(),
				QueryString = new List<HarNameValuePair>(),
				HeadersSize = 200,
				BodySize = 0
			},
			Response = new HarResponse
			{
				Status = 200,
				StatusText = "OK",
				Headers = new List<HarNameValuePair>(),
				Cookies = new List<HarCookie>(),
				Content = new HarContent
				{
					Size = 1000,
					MimeType = "application/json",
					Text = "{\"test\": \"data\"}"
				},
				HeadersSize = 150,
				BodySize = 1000
			},
			Cache = new HarCache(),
			Timings = new HarTimings
			{
				Blocked = 1,
				Dns = 2,
				Connect = 3,
				Send = 4,
				Wait = 5,
				Receive = 6
			}
		};
	}
}