using System.Threading.Tasks;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Models;

namespace DocumentSummarizer.API.Services
{
    /// <summary>
    /// Coordinates the search and summarisation services to provide a single entry point for clients.
    /// </summary>
    public class SearchService : ISearchService
    {
        private readonly IAzureSearchService _azureSearchService;
        private readonly IQueryDocumentService _queryDocumentService;

        public SearchService(IAzureSearchService azureSearchService, IQueryDocumentService queryDocumentService)
        {
            _azureSearchService = azureSearchService;
            _queryDocumentService = queryDocumentService;
        }

        /// <inheritdoc />
        public async Task<ResponseSummary> SummariseAsync(UserQueryRequest request)
        {
            // First perform the search to identify relevant documents.
            var searchResult = await _azureSearchService.SearchAsync(request);

            // In a real implementation you would pass the retrieved documents to your summarisation engine.
            // Here we simply compose a simple summary and list of documents.
            var summary = await _queryDocumentService.GetDocumentSummaryAsync(request);
            summary.SourceDocuments = searchResult.SourceDocuments;
            summary.Summary = $"Search summary for '{request.Query}': {summary.Summary}";
            return summary;
        }
    }
}