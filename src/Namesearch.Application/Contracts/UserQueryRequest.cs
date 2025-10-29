namespace Namesearch.Application.Contracts;

public sealed class UserQueryRequest
{
    public string Query { get; init; } = string.Empty;
    public string SelectedAppId { get; init; } = string.Empty;
    public string SearchType { get; init; } = "semantic_hybrid"; // default aligned with previous impl

    public string SelectedDateField { get; init; } = string.Empty;
    public string SelectedSearchField { get; init; } = string.Empty;
    public string SelectedDuration { get; init; } = string.Empty;

    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
