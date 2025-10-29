using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Namesearch.Application.Contracts;

namespace Namesearch.Application.Abstractions;

/// <summary>
/// Abstraction over the search provider (Azure Cognitive Search implementation lives in Infrastructure).
/// </summary>
public interface IAzureSearchService
{
    Task<IReadOnlyList<ResponseSummary>> SearchAsync(UserQueryRequest request, CancellationToken ct = default);
}
