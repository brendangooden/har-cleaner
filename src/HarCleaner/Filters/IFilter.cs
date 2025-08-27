using HarCleaner.Models;

namespace HarCleaner.Filters;

public interface IFilter
{
	bool ShouldInclude(HarEntry entry);
	string FilterName { get; }
}

public class FilterResult
{
	public bool Include { get; set; }
	public string Reason { get; set; } = string.Empty;
}
