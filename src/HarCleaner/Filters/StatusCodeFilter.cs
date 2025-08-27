using HarCleaner.Models;

namespace HarCleaner.Filters;

public class StatusCodeFilter : IFilter
{
	private readonly int[] _includeStatusCodes;
	private readonly int[] _excludeStatusCodes;

	public string FilterName => "StatusCode";

	public StatusCodeFilter(int[] includeStatusCodes, int[] excludeStatusCodes)
	{
		_includeStatusCodes = includeStatusCodes;
		_excludeStatusCodes = excludeStatusCodes;
	}

	public bool ShouldInclude(HarEntry entry)
	{
		var statusCode = entry.Response.Status;

		// Check excluded status codes
		if (_excludeStatusCodes.Length > 0 && _excludeStatusCodes.Contains(statusCode))
		{
			return false;
		}

		// Check included status codes
		if (_includeStatusCodes.Length > 0 && !_includeStatusCodes.Contains(statusCode))
		{
			return false;
		}

		return true;
	}
}
