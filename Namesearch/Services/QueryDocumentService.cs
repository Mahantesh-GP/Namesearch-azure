using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Models;
using Microsoft.Extensions.Logging;

namespace DocumentSummarizer.API.Services
{
    public class QueryDocumentService : IQueryDocumentService
    {
    private readonly IAzureSearchService _azureSearchService;
        private readonly ILogger<QueryDocumentService> _logger;

        /// <summary>
        /// Constructs a new QueryDocumentService with the necessary clients injected.
        /// </summary>
        public QueryDocumentService(
            IAzureSearchService azureSearchService,
            ILogger<QueryDocumentService> logger)
        {
            _azureSearchService = azureSearchService;
            _logger = logger;
            // TableClient could be injected here if needed; left unused for now
        }

        /// <summary>
        /// Executes a hybrid search using Azure Cognitive Search and returns a list of summaries.
        /// </summary>
        public async Task<List<ResponseSummary>> QueryHybridAsync(UserQueryRequest request)
        {
            _logger.LogInformation("QueryDocumentAsync started with query: {query}", request.Query);

            var results = new List<ResponseSummary>();

            // Perform the hybrid search
            var (searchResults, aliases) = await _azureSearchService.RunHybridSearchAsync(request);
            var resultsList = searchResults.GetResults().ToList();

            // No results found
            if (!resultsList.Any())
            {
                _logger.LogWarning("No relevant documents found for query: {query}", request.Query);
                return results;
            }

            // Build ResponseSummary objects from each search hit
            foreach (var result in resultsList)
            {
                var doc = result.Document;
                var chunk = doc.TryGetValue("fullName", out var fullNameObj) ? fullNameObj?.ToString() : null;

                results.Add(new ResponseSummary
                {
                    Summary = chunk ?? string.Empty,
                    FileName = doc.TryGetValue("sourceId", out var sourceIdObj) ? sourceIdObj?.ToString() ?? string.Empty : string.Empty,
                    FileUrl = doc.TryGetValue("posted", out var postedObj) ? postedObj?.ToString() ?? string.Empty : string.Empty,
                    AppID = doc.TryGetValue("individualFlag", out var individualFlagObj) ? individualFlagObj?.ToString() ?? string.Empty : string.Empty,
                    Parrent_Id = doc.TryGetValue("nameId", out var nameIdObj) ? nameIdObj?.ToString() ?? string.Empty : string.Empty,
                    Score = result.Score.HasValue ? result.Score.Value.ToString("0.00") : string.Empty,
                    folderName = aliases.ToString()
                });
            }

            _logger.LogInformation("QueryDocumentAsync completed successfully for query: {query}", request.Query);
            return results;
        }
    }
}
