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
		// Note: This would require extending the HarEntry model to include Chrome-specific fields
		// For now, this is a placeholder showing the concept

		// In a real implementation, you would clean fields like:
		// - entry._connectionId
		// - entry._initiator
		// - entry._priority
		// - entry._resourceType
		// - entry.response._transferSize
		// - entry.timings._blocked_queueing
		// - entry.timings._workerStart, etc.

		return true;
	}
}
