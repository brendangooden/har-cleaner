using HarCleaner.Models;

namespace HarCleaner.Filters;

public class SizeFilter : IFilter
{
    private readonly int? _minSize;
    private readonly int? _maxSize;

    public string FilterName => "Size";

    public SizeFilter(int? minSize, int? maxSize)
    {
        _minSize = minSize;
        _maxSize = maxSize;
    }

    public bool ShouldInclude(HarEntry entry)
    {
        var responseSize = entry.Response.Content.Size;

        if (_minSize.HasValue && responseSize < _minSize.Value)
            return false;

        if (_maxSize.HasValue && responseSize > _maxSize.Value)
            return false;

        return true;
    }
}
