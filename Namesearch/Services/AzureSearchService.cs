using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using DocumentSummarizer.API.Enum;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Models;
using DocumentSummarizer.API.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentSummarizer.API.Services
{
    public class AzureSearchService : IAzureSearchService
    {
        private readonly IConfiguration _config;
        private readonly AzureSearchOptions? _searchOptions;
        private readonly ILogger<AzureSearchService> _logger;

        /// <summary>
        /// Initializes a new instance of AzureSearchService.
        /// </summary>
        public AzureSearchService(
            IOptions<AzureSearchOptions> searchOptions,
            IConfiguration config,
            ILogger<AzureSearchService> logger)
        {
            _config = config;
            _searchOptions = searchOptions?.Value;
            _logger = logger;
        }

        /// <summary>
        /// Builds a SearchClient based on the search context, pulling the endpoint and API key from configuration.
        /// </summary>
        public SearchClient GetClient(SearchContext context)
        {
            var parts = context.ToString().Split('_');
            var serviceKey = parts[0];
            var indexKey = parts[1];

            var section = _config.GetSection($"AzureSearchServices:{serviceKey}");
            var endpoint = section["Endpoint"];
            var apiKey = section["ApiKey"];
            var indexName = section.GetSection("Indexes")[indexKey];

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(indexName))
            {
                throw new InvalidOperationException($"AzureSearchServices:{serviceKey} configuration is missing required values.");
            }

            return new SearchClient(
                new Uri(endpoint),
                indexName,
                new AzureKeyCredential(apiKey));
        }

        /// <summary>
        /// Performs a hybrid search using Azure Cognitive Search, with filtering and ordering logic.
        /// Returns the search results and an alias string for grouping (aliases come from query suggestions).
        /// </summary>
        public async Task<(SearchResults<SearchDocument> results, string aliases)> RunHybridSearchAsync(UserQueryRequest request)
        {
            _logger.LogInformation("SearchDocument started with query: {query}", request.Query);

            try
            {
                // Prepare search options
                var options = new SearchOptions
                {
                    Size = request.PageSize,
                    Skip = (request.Page - 1) * request.PageSize,
                    QueryType = SearchQueryType.Full,
                    SearchMode = SearchMode.Any,
                    IncludeTotalCount = true
                };

                // Fields to search on
                options.SearchFields.Add("fullname");
                options.SearchFields.Add("fullnameDoubleMet");
                options.SearchFields.Add("fullnameBeider");

                // Order by search score (descending)
                options.OrderBy.Add("search.score() desc");

                // Build filters
                var filters = new List<string>();

                if (!string.IsNullOrWhiteSpace(request.SelectedAppId))
                {
                    filters.Add($"countyid eq '{request.SelectedAppId}'");
                }

                if (!string.IsNullOrWhiteSpace(request.SelectedSearchFiled))
                {
                    filters.Add($"individualflag eq '{request.SelectedSearchFiled}'");
                }
                else
                {
                    filters.Add("(individualflag eq 'I' or individualflag eq 'B')");
                }

                options.Filter = string.Join(" and ", filters);

                // Prefer typed options; fall back to IConfiguration if not provided
                var endpoint = !string.IsNullOrWhiteSpace(_searchOptions?.Endpoint)
                    ? _searchOptions!.Endpoint
                    : _config["AzureSearchServices:Endpoint"];

                var apiKey = !string.IsNullOrWhiteSpace(_searchOptions?.ApiKey)
                    ? _searchOptions!.ApiKey
                    : _config["AzureSearchServices:ApiKey"];

                var indexName = !string.IsNullOrWhiteSpace(_searchOptions?.IndexName)
                    ? _searchOptions!.IndexName
                    : (_config["AzureSearchServices:IndexName"] ?? "hybrid");

                if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
                {
                    throw new InvalidOperationException("AzureSearchServices configuration is missing. Please set AzureSearchServices:Endpoint and AzureSearchServices:ApiKey in appsettings.json.");
                }

                var client = new SearchClient(new Uri(endpoint), indexName, new AzureKeyCredential(apiKey));

                // Execute the search
                var result = await client.SearchAsync<SearchDocument>(request.Query, options);
                return (result.Value, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during search");
                throw;
            }
        }

        // Note: Any Azure OpenAI integration for name variation generation can be added here in the future.
    }
}
