using System.Threading.Tasks;
using DocumentSummarizer.API.Models;

namespace DocumentSummarizer.API.Interfaces
{
    /// <summary>
    /// Combines multiple search‑related services to provide a unified entry point for search operations.
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        /// Performs a search and summarises the results for the user.
        /// </summary>
        /// <param name="request">The user's query request.</param>
        /// <returns>A <see cref="ResponseSummary"/> containing the summarised search results.</returns>
        Task<ResponseSummary> SummariseAsync(UserQueryRequest request);
    }
}