using HarCleaner.Filters;
using HarCleaner.Models;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Filters;

public class PrivacyFilterTests
{
	[Fact]
	public void ShouldInclude_RemoveCookies_ClearsCookiesFromRequestAndResponse()
	{
		// Arrange
		var filter = new PrivacyFilter(removeCookies: true);
		var entry = TestDataHelper.CreateEntryWithCookies("https://example.com/page");

		var originalRequestCookieCount = entry.Request.Cookies.Count;
		var originalResponseCookieCount = entry.Response.Cookies.Count;

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(originalResponseCookieCount > 0); // Verify we had cookies in the response originally
        Assert.True(result); // Privacy filter doesn't exclude entries
		Assert.True(originalRequestCookieCount > 0); // Verify we had cookies originally
		Assert.Empty(entry.Request.Cookies); // Cookies should be cleared
		Assert.Empty(entry.Response.Cookies);

    }

	[Fact]
	public void ShouldInclude_RemoveAuthTokens_RemovesAuthorizationHeaders()
	{
		// Arrange
		var filter = new PrivacyFilter(removeAuthTokens: true);
		var entry = TestDataHelper.CreateEntryWithCookies("https://example.com/api");

		var originalHeaderCount = entry.Request.Headers.Count;

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.True(originalHeaderCount > 0);
		Assert.DoesNotContain(entry.Request.Headers, h => h.Name.ToLower().Contains("authorization"));
	}

	[Fact]
	public void ShouldInclude_RemovePersonalIdentifiers_RedactsQueryParameters()
	{
		// Arrange
		var filter = new PrivacyFilter(removePersonalIdentifiers: true);
		var entry = TestDataHelper.CreateTestEntry("https://example.com/page");

		// Add sensitive query parameters
		entry.Request.QueryString.Add(new HarNameValuePair { Name = "session_id", Value = "abc123" });
		entry.Request.QueryString.Add(new HarNameValuePair { Name = "user_token", Value = "xyz789" });
		entry.Request.QueryString.Add(new HarNameValuePair { Name = "safe_param", Value = "ok" });

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		var sessionParam = entry.Request.QueryString.FirstOrDefault(q => string.Equals(q.Name, "session_id", StringComparison.OrdinalIgnoreCase));
		var tokenParam = entry.Request.QueryString.FirstOrDefault(q => string.Equals(q.Name, "user_token", StringComparison.OrdinalIgnoreCase));
		var safeParam = entry.Request.QueryString.FirstOrDefault(q => string.Equals(q.Name, "safe_param", StringComparison.OrdinalIgnoreCase));

		Assert.Equal("[REDACTED]", sessionParam?.Value);
		Assert.Equal("[REDACTED]", tokenParam?.Value);
		Assert.Equal("ok", safeParam?.Value); // Safe param should remain unchanged
	}

	[Fact]
	public void ShouldInclude_RemoveTrackingHeaders_RemovesUserAgentAndLanguage()
	{
		// Arrange
		var filter = new PrivacyFilter(removeTrackingHeaders: true);
		var entry = TestDataHelper.CreateTestEntry("https://example.com/page");

		// Add tracking headers
		entry.Request.Headers.Add(new HarNameValuePair { Name = "User-Agent", Value = "Mozilla/5.0..." });
		entry.Request.Headers.Add(new HarNameValuePair { Name = "Accept-Language", Value = "en-US,en;q=0.9" });
		entry.Request.Headers.Add(new HarNameValuePair { Name = "Accept", Value = "text/html" });

		var originalHeaderCount = entry.Request.Headers.Count;

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.True(originalHeaderCount > 0);
		Assert.DoesNotContain(entry.Request.Headers, h => h.Name.Equals("User-Agent", StringComparison.OrdinalIgnoreCase));
		Assert.DoesNotContain(entry.Request.Headers, h => h.Name.Equals("Accept-Language", StringComparison.OrdinalIgnoreCase));
		Assert.Contains(entry.Request.Headers, h => h.Name.Equals("Accept", StringComparison.OrdinalIgnoreCase)); // Non-tracking header should remain
	}

	[Fact]
	public void ShouldInclude_AllPrivacyOptionsEnabled_CleansAllSensitiveData()
	{
		// Arrange
		var filter = new PrivacyFilter(
			removeCookies: true,
			removeAuthTokens: true,
			removePersonalIdentifiers: true,
			removeTrackingHeaders: true);

		var entry = TestDataHelper.CreateEntryWithCookies("https://example.com/api");
		entry.Request.Headers.Add(new HarNameValuePair { Name = "User-Agent", Value = "Mozilla/5.0..." });
		entry.Request.QueryString.Add(new HarNameValuePair { Name = "session", Value = "abc123" });

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Empty(entry.Request.Cookies);
		Assert.Empty(entry.Response.Cookies);
		Assert.DoesNotContain(entry.Request.Headers, h => h.Name.ToLower().Contains("authorization"));
		Assert.DoesNotContain(entry.Request.Headers, h => h.Name.Equals("User-Agent", StringComparison.OrdinalIgnoreCase));

		var sessionParam = entry.Request.QueryString.FirstOrDefault(q => string.Equals(q.Name, "session", StringComparison.OrdinalIgnoreCase));
		Assert.NotNull(sessionParam);
		Assert.Equal("[REDACTED]", sessionParam.Value);
	}

	[Fact]
	public void ShouldInclude_NoPrivacyOptionsEnabled_LeavesDataUnchanged()
	{
		// Arrange
		var filter = new PrivacyFilter();
		var entry = TestDataHelper.CreateEntryWithCookies("https://example.com/page");

		var originalCookieCount = entry.Request.Cookies.Count;
		var originalHeaderCount = entry.Request.Headers.Count;

		// Act
		var result = filter.ShouldInclude(entry);

		// Assert
		Assert.True(result);
		Assert.Equal(originalCookieCount, entry.Request.Cookies.Count);
		Assert.Equal(originalHeaderCount, entry.Request.Headers.Count);
	}
}
