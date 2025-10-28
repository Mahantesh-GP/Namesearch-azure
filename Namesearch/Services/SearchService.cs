using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DocumentSummarizer.API.Enum;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Models;
using Microsoft.Extensions.Logging;

namespace DocumentSummarizer.API.Services
{
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly IQueryDocumentService _queryClient;

        public SearchService(
            ILogger<SearchService> logger,
            IQueryDocumentService queryClient)
        {
            _logger = logger;
            _queryClient = queryClient;
        }

        public async Task<List<ResponseSummary>> QueryDocumentAsync(UserQueryRequest request)
        {
            // Determine which search approach to invoke based on searchType
            switch (request.searchType)
            {
                case var s when s == SearchType.semantic_hybrid.ToString():
                    return await _queryClient.QueryHybridAsync(request);

                default:
                    throw new InvalidOperationException("Unsupported search type");
            }
        }
    }
}
