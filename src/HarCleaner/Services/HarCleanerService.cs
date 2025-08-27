using HarCleaner.Filters;
using HarCleaner.Models;

namespace HarCleaner.Services;

public class HarCleanerService
{
	private readonly List<IFilter> _filters = new();

	public void AddFilter(IFilter filter)
	{
		_filters.Add(filter);
	}

	public CleaningResult Clean(HarFile harFile, bool verbose = false)
	{
		var originalCount = harFile.Log.Entries.Count;
		var filteredEntries = new List<HarEntry>();
		var excludedEntries = new List<ExcludedEntry>();

		foreach (var entry in harFile.Log.Entries)
		{
			var shouldInclude = true;
			var excludeReasons = new List<string>();

			foreach (var filter in _filters)
			{
				if (!filter.ShouldInclude(entry))
				{
					shouldInclude = false;
					excludeReasons.Add(filter.FilterName);
				}
			}

			if (shouldInclude)
			{
				filteredEntries.Add(entry);
			}
			else if (verbose)
			{
				excludedEntries.Add(new ExcludedEntry
				{
					Url = entry.Request.Url,
					Method = entry.Request.Method,
					Status = entry.Response.Status,
					Reasons = excludeReasons
				});
			}
		}

		// Create a new HAR file with filtered entries
		var cleanedHarFile = new HarFile
		{
			Log = new HarLog
			{
				Version = harFile.Log.Version,
				Creator = harFile.Log.Creator,
				Browser = harFile.Log.Browser,
				Pages = harFile.Log.Pages,
				Entries = filteredEntries,
				Comment = harFile.Log.Comment
			}
		};

		return new CleaningResult
		{
			CleanedHarFile = cleanedHarFile,
			OriginalCount = originalCount,
			FilteredCount = filteredEntries.Count,
			ExcludedEntries = excludedEntries
		};
	}
}

public class CleaningResult
{
	public HarFile CleanedHarFile { get; set; } = new();
	public int OriginalCount { get; set; }
	public int FilteredCount { get; set; }
	public List<ExcludedEntry> ExcludedEntries { get; set; } = new();

	public int RemovedCount => OriginalCount - FilteredCount;
	public double RemovalPercentage => OriginalCount > 0 ? (double)RemovedCount / OriginalCount * 100 : 0;
}

public class ExcludedEntry
{
	public string Url { get; set; } = string.Empty;
	public string Method { get; set; } = string.Empty;
	public int Status { get; set; }
	public List<string> Reasons { get; set; } = new();
}
