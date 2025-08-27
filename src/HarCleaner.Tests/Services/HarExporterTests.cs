using HarCleaner.Services;
using HarCleaner.Tests.Helpers;
using Xunit;
using System.Text.Json;

namespace HarCleaner.Tests.Services;

public class HarExporterTests
{
	private readonly string _testOutputPath = Path.Combine(Path.GetTempPath(), "output_test.har");

	[Fact]
	public async Task SaveAsync_ValidHarFile_CreatesFile()
	{
		// Arrange
		var harFile = TestDataHelper.CreateTestHarFile();
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/test"));

		var exporter = new HarExporter();

		try
		{
			// Act
			await exporter.SaveAsync(harFile, _testOutputPath);

			// Assert
			Assert.True(File.Exists(_testOutputPath));

			var savedContent = await File.ReadAllTextAsync(_testOutputPath);
			var deserializedHar = JsonSerializer.Deserialize<HarCleaner.Models.HarFile>(savedContent);

			Assert.NotNull(deserializedHar);
			Assert.Equal("1.2", deserializedHar.Log.Version);
			Assert.Single(deserializedHar.Log.Entries);
		}
		finally
		{
			// Cleanup
			if (File.Exists(_testOutputPath))
			{
				File.Delete(_testOutputPath);
			}
		}
	}

	[Fact]
	public async Task SaveAsync_CreatesDirectoryIfNotExists()
	{
		// Arrange
		var nestedPath = Path.Combine(Path.GetTempPath(), "test_nested", "subfolder", "output.har");
		var harFile = TestDataHelper.CreateTestHarFile();
		var exporter = new HarExporter();

		try
		{
			// Act
			await exporter.SaveAsync(harFile, nestedPath);

			// Assert
			Assert.True(File.Exists(nestedPath));
			Assert.True(Directory.Exists(Path.GetDirectoryName(nestedPath)));
		}
		finally
		{
			// Cleanup
			var directory = Path.GetDirectoryName(nestedPath);
			if (Directory.Exists(directory))
			{
				Directory.Delete(directory, true);
			}
		}
	}

	[Fact]
	public void Save_SynchronousVersion_WorksCorrectly()
	{
		// Arrange
		var harFile = TestDataHelper.CreateTestHarFile();
		var exporter = new HarExporter();

		try
		{
			// Act
			exporter.Save(harFile, _testOutputPath);

			// Assert
			Assert.True(File.Exists(_testOutputPath));
		}
		finally
		{
			// Cleanup
			if (File.Exists(_testOutputPath))
			{
				File.Delete(_testOutputPath);
			}
		}
	}

	[Fact]
	public async Task SaveAsync_ProducesValidJson()
	{
		// Arrange
		var harFile = TestDataHelper.CreateTestHarFile();
		harFile.Log.Entries.Add(TestDataHelper.CreateTestEntry("https://example.com/api"));
		harFile.Log.Entries.Add(TestDataHelper.CreateXhrEntry("https://example.com/xhr"));

		var exporter = new HarExporter();

		try
		{
			// Act
			await exporter.SaveAsync(harFile, _testOutputPath);

			// Assert
			var savedContent = await File.ReadAllTextAsync(_testOutputPath);

			// Verify it's valid JSON by deserializing
			var deserializedHar = JsonSerializer.Deserialize<HarCleaner.Models.HarFile>(savedContent);
			Assert.NotNull(deserializedHar);
			Assert.Equal(2, deserializedHar.Log.Entries.Count);

			// Verify JSON is properly formatted (indented)
			Assert.Contains("\n", savedContent, StringComparison.OrdinalIgnoreCase); // Should have newlines for indentation
			Assert.Contains("  ", savedContent, StringComparison.OrdinalIgnoreCase); // Should have indentation spaces
		}
		finally
		{
			// Cleanup
			if (File.Exists(_testOutputPath))
			{
				File.Delete(_testOutputPath);
			}
		}
	}
}
