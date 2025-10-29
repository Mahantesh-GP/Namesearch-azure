using System.Collections.Generic;

namespace Namesearch.Application.Contracts;

/// <summary>
/// Wrapper response containing search results and the AI-generated nickname variations
/// </summary>
public sealed class SearchResponse
{
    /// <summary>
    /// The original query entered by the user
    /// </summary>
    public string OriginalQuery { get; init; } = string.Empty;

    /// <summary>
    /// AI-generated nickname variations that were searched (includes original query)
    /// </summary>
    public IReadOnlyList<string> SearchedVariations { get; init; } = new List<string>();

    /// <summary>
    /// Search results matching the enriched query
    /// </summary>
    public IReadOnlyList<ResponseSummary> Results { get; init; } = new List<ResponseSummary>();

    /// <summary>
    /// Total number of results found
    /// </summary>
    public int TotalCount { get; init; }
}
