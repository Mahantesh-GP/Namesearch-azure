using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Models;

namespace DocumentSummarizer.API.Services
{
    /// <summary>
    /// Stub implementation of <see cref="IAzureSearchService"/>. In a production system this class
    /// would connect to Azure Cognitive Search and return ranked results for the user's query.
    /// </summary>
    public class AzureSearchService : IAzureSearchService
    {
        public Task<ResponseSummary> SearchAsync(UserQueryRequest request)
        {
            // This stub simply echoes the query back. Replace with actual search logic.
            var summary = new ResponseSummary
            {
                Summary = $"Simulated search results for query '{request.Query}'",
                SourceDocuments = new List<string> { "doc1", "doc2", "doc3" }
            };
            return Task.FromResult(summary);
        }
    }
}