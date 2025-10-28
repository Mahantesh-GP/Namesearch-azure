using System.Threading.Tasks;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DocumentSummarizer.API.Models;

namespace DocumentSummarizer.API.Interfaces
{
    /// <summary>
    /// Encapsulates the logic required to perform searches against an Azure Cognitive Search index.
    /// Actual implementations will connect to Azure and return results based on the user's query.
    /// </summary>
    public interface IAzureSearchService
    {
        /// <summary>
        /// Executes a search on the configured search service and returns a summary of results.
        /// </summary>
        /// <param name="request">The user's search request containing the query and optional filters.</param>
        /// <returns>A <see cref="ResponseSummary"/> describing the search results.</returns>
        Task<(SearchResults<SearchDocument> results, string aliases)> RunHybridSearchAsync(UserQueryRequest request);
    }
}