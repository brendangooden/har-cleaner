using HarCleaner.Services;
using HarCleaner.Models;
using HarCleaner.Tests.Helpers;
using Xunit;
using System.Text.Json;

namespace HarCleaner.Tests.Services;

public class HarLoaderTests
{
	private readonly string _testFilePath = Path.Combine(Path.GetTempPath(), "test.har");

	[Fact]
	public async Task LoadAsync_ValidHarFile_ReturnsHarFile()
	{
		// Arrange
		var harFile = TestDataHelper.CreateTestHarFile();
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/test"));

		var json = JsonSerializer.Serialize(harFile, new JsonSerializerOptions { WriteIndented = true });
		await File.WriteAllTextAsync(_testFilePath, json);

		var loader = new HarLoader();

		try
		{
			// Act
			var result = await loader.LoadAsync(_testFilePath);

			// Assert
			Assert.NotNull(result);
			Assert.NotNull(result.Log);
			Assert.Equal("1.2", result.Log.Version);
			Assert.Single(result.Log.Entries);
		}
		finally
		{
			// Cleanup
			if (File.Exists(_testFilePath))
			{
				File.Delete(_testFilePath);
			}
		}
	}

	[Fact]
	public async Task LoadAsync_FileNotFound_ThrowsFileNotFoundException()
	{
		// Arrange
		var loader = new HarLoader();
		var nonExistentFile = Path.Combine(Path.GetTempPath(), "nonexistent.har");

		// Act & Assert
		await Assert.ThrowsAsync<FileNotFoundException>(() => loader.LoadAsync(nonExistentFile));
	}

	[Fact]
	public async Task LoadAsync_InvalidJson_ThrowsInvalidOperationException()
	{
		// Arrange
		var invalidJson = "{ invalid json content";
		await File.WriteAllTextAsync(_testFilePath, invalidJson);

		var loader = new HarLoader();

		try
		{
			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadAsync(_testFilePath));
		}
		finally
		{
			// Cleanup
			if (File.Exists(_testFilePath))
			{
				File.Delete(_testFilePath);
			}
		}
	}

	[Fact]
	public async Task LoadAsync_EmptyLogProperty_ThrowsInvalidOperationException()
	{
		// Arrange
		var invalidHar = "{ \"log\": null }";
		await File.WriteAllTextAsync(_testFilePath, invalidHar);

		var loader = new HarLoader();

		try
		{
			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => loader.LoadAsync(_testFilePath));
		}
		finally
		{
			// Cleanup
			if (File.Exists(_testFilePath))
			{
				File.Delete(_testFilePath);
			}
		}
	}

	[Fact]
	public void Load_SynchronousVersion_WorksCorrectly()
	{
		// Arrange
		var harFile = TestDataHelper.CreateTestHarFile();
		var json = JsonSerializer.Serialize(harFile, new JsonSerializerOptions { WriteIndented = true });
		File.WriteAllText(_testFilePath, json);

		var loader = new HarLoader();

		try
		{
			// Act
			var result = loader.Load(_testFilePath);

			// Assert
			Assert.NotNull(result);
			Assert.NotNull(result.Log);
			Assert.Equal("1.2", result.Log.Version);
		}
		finally
		{
			// Cleanup
			if (File.Exists(_testFilePath))
			{
				File.Delete(_testFilePath);
			}
		}
	}
}
