using HarCleaner.Filters;
using HarCleaner.Tests.Helpers;
using Xunit;

namespace HarCleaner.Tests.Filters;

public class ContentFilterTests
{
    [Fact]
    public void ShouldInclude_RemoveResponseContent_ClearsResponseText()
    {
        // Arrange
        var filter = new ContentFilter(removeResponseContent: true);
        var entry = TestDataHelper.CreateEntryWithContent("https://example.com/page", "<html><body>Test content</body></html>");

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Null(entry.Response.Content.Text);
    }

    [Fact]
    public void ShouldInclude_RemoveRequestContent_ClearsPostData()
    {
        // Arrange
        var filter = new ContentFilter(removeRequestContent: true);
        var entry = TestDataHelper.CreateTestEntry("https://example.com/api", "POST");
        entry.Request.PostData = new Models.HarPostData
        {
            MimeType = "application/json",
            Text = "{\"key\":\"value\"}"
        };

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Null(entry.Request.PostData.Text);
        Assert.Empty(entry.Request.PostData.Params);
    }

    [Fact]
    public void ShouldInclude_MaxContentSize_ReplacesLargeContent()
    {
        // Arrange
        var filter = new ContentFilter(maxContentSize: 100);
        var largeContent = new string('x', 500);
        var entry = TestDataHelper.CreateEntryWithContent("https://example.com/large", largeContent);

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Contains("[CONTENT REMOVED - Size:", entry.Response.Content.Text);
    }

    [Fact]
    public void ShouldInclude_ExcludeContentTypes_RemovesSpecificMimeTypes()
    {
        // Arrange
        var filter = new ContentFilter(excludeContentTypes: new[] { "image", "video" });
        var imageEntry = TestDataHelper.CreateEntryWithContent("https://example.com/image.png", "binary_image_data", "image/png");
        var textEntry = TestDataHelper.CreateEntryWithContent("https://example.com/page.html", "<html>content</html>", "text/html");

        // Act
        var imageResult = filter.ShouldInclude(imageEntry);
        var textResult = filter.ShouldInclude(textEntry);

        // Assert
        Assert.True(imageResult);
        Assert.True(textResult);
        Assert.Contains("[CONTENT REMOVED - Type:", imageEntry.Response.Content.Text);
        Assert.Equal("<html>content</html>", textEntry.Response.Content.Text); // Text content should remain
    }

    [Fact]
    public void ShouldInclude_RemoveBase64Content_DetectsAndRemovesBase64()
    {
        // Arrange
        var filter = new ContentFilter(removeBase64Content: true);
        var base64Content = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==";
        var regularContent = "This is regular text content";
        
        var base64Entry = TestDataHelper.CreateEntryWithContent("https://example.com/image", base64Content);
        var textEntry = TestDataHelper.CreateEntryWithContent("https://example.com/text", regularContent);

        // Act
        var base64Result = filter.ShouldInclude(base64Entry);
        var textResult = filter.ShouldInclude(textEntry);

        // Assert
        Assert.True(base64Result);
        Assert.True(textResult);
        Assert.Equal("[BASE64 CONTENT REMOVED]", base64Entry.Response.Content.Text);
        Assert.Equal(regularContent, textEntry.Response.Content.Text);
    }

    [Fact]
    public void ShouldInclude_NoContentFilters_LeavesContentUnchanged()
    {
        // Arrange
        var filter = new ContentFilter();
        var originalContent = "<html><body>Original content</body></html>";
        var entry = TestDataHelper.CreateEntryWithContent("https://example.com/page", originalContent);

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(originalContent, entry.Response.Content.Text);
    }

    [Fact]
    public void ShouldInclude_SmallContentUnderLimit_RemainsUnchanged()
    {
        // Arrange
        var filter = new ContentFilter(maxContentSize: 1000);
        var smallContent = "Small content";
        var entry = TestDataHelper.CreateEntryWithContent("https://example.com/small", smallContent);

        // Act
        var result = filter.ShouldInclude(entry);

        // Assert
        Assert.True(result);
        Assert.Equal(smallContent, entry.Response.Content.Text);
    }
}
