using System.Text.Json;
using HarCleaner.Models;

namespace HarCleaner.Services;

public class HarExporter
{
	public async Task SaveAsync(HarFile harFile, string filePath)
	{
		try
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase
			};

			var jsonContent = JsonSerializer.Serialize(harFile, options);

			// Ensure the directory exists
			var directory = Path.GetDirectoryName(filePath);
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			await File.WriteAllTextAsync(filePath, jsonContent);
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException($"Failed to save HAR file: {ex.Message}", ex);
		}
	}

	public void Save(HarFile harFile, string filePath)
	{
		SaveAsync(harFile, filePath).GetAwaiter().GetResult();
	}
}
