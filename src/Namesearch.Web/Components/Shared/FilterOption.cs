namespace Namesearch.Web.Components.Shared;

public class FilterOption<TValue>
{
    public TValue Value { get; set; } = default!;
    public string Text { get; set; } = string.Empty;
}
