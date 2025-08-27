using System.Text.Json;
using HarCleaner.Models;

namespace HarCleaner.Services;

public class HarLoader
{
    public async Task<HarFile> LoadAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"HAR file not found: {filePath}");

        try
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            };

            var harFile = JsonSerializer.Deserialize<HarFile>(jsonContent, options);
            
            if (harFile?.Log == null)
                throw new InvalidOperationException("Invalid HAR file format");

            return harFile;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse HAR file: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to load HAR file: {ex.Message}", ex);
        }
    }

    public HarFile Load(string filePath)
    {
        return LoadAsync(filePath).GetAwaiter().GetResult();
    }
}
