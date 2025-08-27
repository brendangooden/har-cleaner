using CommandLine;
using HarCleaner.Filters;
using HarCleaner.Models;
using HarCleaner.Services;

namespace HarCleaner;

public static class Program
{
	public static async Task<int> Main(string[] args)
	{
		return await Parser.Default.ParseArguments<FilterOptions>(args)
			.MapResult(
				async (FilterOptions opts) => await RunCleaningAsync(opts),
				errs => Task.FromResult(1)
			);
	}

	public static async Task<int> RunCleaningAsync(FilterOptions options)
	{
		try
		{
			Console.WriteLine("HAR Cleaner v1.0");
			Console.WriteLine($"Input file: {options.InputFile}");
			Console.WriteLine($"Output file: {options.OutputFile}");
			Console.WriteLine($"Output type: {options.OutputType}");
			Console.WriteLine();

			// Validate input file
			if (!File.Exists(options.InputFile))
			{
				Console.WriteLine($"Error: Input file '{options.InputFile}' not found.");
				return 1;
			}

			// Validate output type
			if (!options.OutputType.Equals("har", StringComparison.OrdinalIgnoreCase) &&
				!options.OutputType.Equals("ml-ingest", StringComparison.OrdinalIgnoreCase))
			{
				Console.WriteLine($"Error: Invalid output type '{options.OutputType}'. Must be 'har' or 'ml-ingest'.");
				return 1;
			}

			// Load HAR file
			Console.WriteLine("Loading HAR file...");
			var loader = new HarLoader();
			var harFile = await loader.LoadAsync(options.InputFile);

			Console.WriteLine($"Loaded {harFile.Log.Entries.Count} entries");

			// Set up filters
			var cleaner = new HarCleanerService();
			var filtersApplied = new List<string>();

			// Add request type filter
			if (options.ExcludeTypesList.Length > 0 || options.IncludeTypesList.Length > 0)
			{
				cleaner.AddFilter(new RequestTypeFilter(options.IncludeTypesList, options.ExcludeTypesList));
				if (options.ExcludeTypesList.Length > 0)
				{
					filtersApplied.Add($"Exclude types: {string.Join(", ", options.ExcludeTypesList)}");
				}

				if (options.IncludeTypesList.Length > 0)
				{
					filtersApplied.Add($"Include types: {string.Join(", ", options.IncludeTypesList)}");
				}
			}

			// Add request method filter
			if (options.XhrOnly || options.IncludeMethodsList.Length > 0 || options.ExcludeMethodsList.Length > 0)
			{
				cleaner.AddFilter(new RequestMethodFilter(options.XhrOnly, options.IncludeMethodsList, options.ExcludeMethodsList));
				if (options.XhrOnly)
				{
					filtersApplied.Add("XHR/AJAX only");
				}

				if (options.IncludeMethodsList.Length > 0)
				{
					filtersApplied.Add($"Include methods: {string.Join(", ", options.IncludeMethodsList)}");
				}

				if (options.ExcludeMethodsList.Length > 0)
				{
					filtersApplied.Add($"Exclude methods: {string.Join(", ", options.ExcludeMethodsList)}");
				}
			}

			// Add URL filter
			if (options.IncludeUrlPatternsList.Length > 0 || options.ExcludeUrlPatternsList.Length > 0)
			{
				cleaner.AddFilter(new UrlFilter(options.IncludeUrlPatternsList, options.ExcludeUrlPatternsList));
				if (options.IncludeUrlPatternsList.Length > 0)
				{
					filtersApplied.Add($"Include URL patterns: {string.Join(", ", options.IncludeUrlPatternsList)}");
				}

				if (options.ExcludeUrlPatternsList.Length > 0)
				{
					filtersApplied.Add($"Exclude URL patterns: {string.Join(", ", options.ExcludeUrlPatternsList)}");
				}
			}

			// Add status code filter
			if (options.IncludeStatusCodesList.Length > 0 || options.ExcludeStatusCodesList.Length > 0)
			{
				cleaner.AddFilter(new StatusCodeFilter(options.IncludeStatusCodesList, options.ExcludeStatusCodesList));
				if (options.IncludeStatusCodesList.Length > 0)
				{
					filtersApplied.Add($"Include status codes: {string.Join(", ", options.IncludeStatusCodesList)}");
				}

				if (options.ExcludeStatusCodesList.Length > 0)
				{
					filtersApplied.Add($"Exclude status codes: {string.Join(", ", options.ExcludeStatusCodesList)}");
				}
			}

			// Add size filter
			if (options.MinSize.HasValue || options.MaxSize.HasValue)
			{
				cleaner.AddFilter(new SizeFilter(options.MinSize, options.MaxSize));
				if (options.MinSize.HasValue)
				{
					filtersApplied.Add($"Min size: {options.MinSize} bytes");
				}

				if (options.MaxSize.HasValue)
				{
					filtersApplied.Add($"Max size: {options.MaxSize} bytes");
				}
			}

			// Add privacy filter
			if (options.RemoveCookies || options.RemoveAuthTokens || options.RemovePersonalIdentifiers || options.RemoveTrackingHeaders)
			{
				cleaner.AddFilter(new PrivacyFilter(
					options.RemoveCookies,
					options.RemoveAuthTokens,
					options.RemovePersonalIdentifiers,
					options.RemoveTrackingHeaders));

				if (options.RemoveCookies)
				{
					filtersApplied.Add("Remove cookies");
				}

				if (options.RemoveAuthTokens)
				{
					filtersApplied.Add("Remove auth tokens");
				}

				if (options.RemovePersonalIdentifiers)
				{
					filtersApplied.Add("Remove personal identifiers");
				}

				if (options.RemoveTrackingHeaders)
				{
					filtersApplied.Add("Remove tracking headers");
				}
			}

			// Add content filter
			if (options.RemoveResponseContent || options.RemoveRequestContent || options.RemoveBase64Content ||
				options.MaxContentSize.HasValue || options.ExcludeContentTypesList.Length > 0)
			{
				cleaner.AddFilter(new ContentFilter(
					options.RemoveResponseContent,
					options.RemoveRequestContent,
					options.RemoveBase64Content,
					options.MaxContentSize,
					options.ExcludeContentTypesList));

				if (options.RemoveResponseContent)
				{
					filtersApplied.Add("Remove response content");
				}

				if (options.RemoveRequestContent)
				{
					filtersApplied.Add("Remove request content");
				}

				if (options.RemoveBase64Content)
				{
					filtersApplied.Add("Remove base64 content");
				}

				if (options.MaxContentSize.HasValue)
				{
					filtersApplied.Add($"Max content size: {options.MaxContentSize} bytes");
				}

				if (options.ExcludeContentTypesList.Length > 0)
				{
					filtersApplied.Add($"Exclude content types: {string.Join(", ", options.ExcludeContentTypesList)}");
				}
			}

			// Add Chrome data filter
			if (options.RemoveChromeData)
			{
				cleaner.AddFilter(new ChromeDataFilter(true, true, true, true, true, true));
				filtersApplied.Add("Remove Chrome DevTools data");
			}

			if (filtersApplied.Count == 0)
			{
				Console.WriteLine("Warning: No filters specified. Output will be identical to input.");
			}
			else
			{
				Console.WriteLine("Applied filters:");
				foreach (var filter in filtersApplied)
				{
					Console.WriteLine($"  - {filter}");
				}
			}

			Console.WriteLine();

			// Clean the HAR file
			Console.WriteLine("Applying filters...");
			var result = cleaner.Clean(harFile, options.Verbose);

			// Display results
			Console.WriteLine($"Original entries: {result.OriginalCount}");
			Console.WriteLine($"Filtered entries: {result.FilteredCount}");
			Console.WriteLine($"Removed entries: {result.RemovedCount} ({result.RemovalPercentage:F1}%)");

			if (options.Verbose && result.ExcludedEntries.Count > 0)
			{
				Console.WriteLine();
				Console.WriteLine("Excluded entries:");
				foreach (var excluded in result.ExcludedEntries.Take(10)) // Show first 10
				{
					Console.WriteLine($"  {excluded.Method} {excluded.Status} {excluded.Url}");
					Console.WriteLine($"    Reasons: {string.Join(", ", excluded.Reasons)}");
				}
				if (result.ExcludedEntries.Count > 10)
				{
					Console.WriteLine($"  ... and {result.ExcludedEntries.Count - 10} more");
				}
			}

			// Save the cleaned HAR file
			if (!options.DryRun)
			{
				Console.WriteLine();
				Console.WriteLine($"Saving cleaned {options.OutputType.ToUpper()} file...");

				if (options.OutputType.Equals("ml-ingest", StringComparison.OrdinalIgnoreCase))
				{
					var mlExporter = new MlIngestExporter();
					await mlExporter.SaveAsync(result.CleanedHarFile, options.OutputFile);
				}
				else
				{
					var exporter = new HarExporter();
					await exporter.SaveAsync(result.CleanedHarFile, options.OutputFile);
				}

				Console.WriteLine($"Saved to: {options.OutputFile}");
			}
			else
			{
				Console.WriteLine();
				Console.WriteLine("Dry run mode - no file saved.");
			}

			Console.WriteLine();
			Console.WriteLine("✓ Cleaning completed successfully!");
			return 0;
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
			if (options.Verbose)
			{
				Console.WriteLine($"Stack trace: {ex.StackTrace}");
			}
			return 1;
		}
	}
}
