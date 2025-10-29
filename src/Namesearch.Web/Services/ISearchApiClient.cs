using Namesearch.Web.Models;

namespace Namesearch.Web.Services;

public interface ISearchApiClient
{
    Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
}
