using HarCleaner.Models;

namespace HarCleaner.Filters;

public class ChromeDataFilter : IFilter
{
	private readonly bool _removeConnectionIds;
	private readonly bool _removeInitiatorData;
	private readonly bool _removePriorityData;
	private readonly bool _removeResourceTypeData;
	private readonly bool _removeInternalTimings;
	private readonly bool _removeTransferSizes;

	public string FilterName => "ChromeData";

	public ChromeDataFilter(bool removeConnectionIds = false, bool removeInitiatorData = false,
						   bool removePriorityData = false, bool removeResourceTypeData = false,
						   bool removeInternalTimings = false, bool removeTransferSizes = false)
	{
		_removeConnectionIds = removeConnectionIds;
		_removeInitiatorData = removeInitiatorData;
		_removePriorityData = removePriorityData;
		_removeResourceTypeData = removeResourceTypeData;
		_removeInternalTimings = removeInternalTimings;
		_removeTransferSizes = removeTransferSizes;
	}

	public bool ShouldInclude(HarEntry entry)
	{
		// This filter cleans Chrome-specific data but doesn't exclude entries
		
		if (_removeConnectionIds)
		{
			entry.ConnectionId = null;
		}

		if (_removeInitiatorData)
		{
			entry.Initiator = null;
		}

		if (_removePriorityData)
		{
			entry.Priority = null;
		}

		if (_removeResourceTypeData)
		{
			entry.ResourceType = null;
		}

		if (_removeInternalTimings)
		{
			RemoveInternalTimings(entry);
		}

		if (_removeTransferSizes)
		{
			RemoveTransferSizes(entry);
		}

		return true;
	}

	private static void RemoveInternalTimings(HarEntry entry)
	{
		// Remove Chrome-specific timing fields by setting them to null
		// These are fields that Chrome DevTools adds beyond the HAR spec
		
		// Remove comment field that might contain Chrome-specific timing data
		if (!string.IsNullOrEmpty(entry.Timings.Comment) && 
		    entry.Timings.Comment.Contains("queueing", StringComparison.OrdinalIgnoreCase))
		{
			entry.Timings.Comment = null;
		}

		// Remove entry-level comment that might contain Chrome-specific data
		if (!string.IsNullOrEmpty(entry.Comment) &&
		    (entry.Comment.Contains("devtools", StringComparison.OrdinalIgnoreCase) ||
		     entry.Comment.Contains("chrome", StringComparison.OrdinalIgnoreCase)))
		{
			entry.Comment = null;
		}
	}

	private static void RemoveTransferSizes(HarEntry entry)
	{
		// Remove Chrome-specific transfer size data
		// Chrome DevTools sometimes adds transfer size information beyond standard HAR
		
		// Remove response headers that contain Chrome-specific transfer size info
		var headersToRemove = new List<string>
		{
			"x-devtools-",
			"x-chrome-",
			"x-transfer-size"
		};

		for (int i = entry.Response.Headers.Count - 1; i >= 0; i--)
		{
			var header = entry.Response.Headers[i];
			var headerName = header.Name.ToLowerInvariant();
			
			if (headersToRemove.Any(prefix => headerName.StartsWith(prefix, StringComparison.Ordinal)))
			{
				entry.Response.Headers.RemoveAt(i);
			}
		}

		// Also check request headers for Chrome-specific transfer data
		for (int i = entry.Request.Headers.Count - 1; i >= 0; i--)
		{
			var header = entry.Request.Headers[i];
			var headerName = header.Name.ToLowerInvariant();
			
			if (headersToRemove.Any(prefix => headerName.StartsWith(prefix, StringComparison.Ordinal)))
			{
				entry.Request.Headers.RemoveAt(i);
			}
		}
	}
}
