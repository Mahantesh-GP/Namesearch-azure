using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Namesearch.Application.Abstractions;
using Namesearch.Application.Contracts;
using Namesearch.Infrastructure.Configuration;

namespace Namesearch.Infrastructure.Search;

public sealed class AzureSearchService : IAzureSearchService
{
    private readonly SearchClient _client;
    private readonly ILogger<AzureSearchService> _logger;

    public AzureSearchService(IOptions<AzureSearchOptions> options, ILogger<AzureSearchService> logger)
    {
        var opt = options.Value ?? throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrWhiteSpace(opt.Endpoint) || string.IsNullOrWhiteSpace(opt.ApiKey) || string.IsNullOrWhiteSpace(opt.IndexName))
        {
            throw new InvalidOperationException("AzureSearchOptions must include Endpoint, ApiKey, and IndexName.");
        }

        _client = new SearchClient(new Uri(opt.Endpoint), opt.IndexName, new AzureKeyCredential(opt.ApiKey));
        _logger = logger;
    }

    public async Task<IReadOnlyList<ResponseSummary>> SearchAsync(UserQueryRequest request, CancellationToken ct = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Query)) return Array.Empty<ResponseSummary>();

        var options = BuildSearchOptions(request);
        var results = await _client.SearchAsync<SearchDocument>(request.Query, options, ct);

        var list = new List<ResponseSummary>();
        await foreach (var res in results.Value.GetResultsAsync().WithCancellation(ct))
        {
            var doc = res.Document;
            list.Add(new ResponseSummary
            {
                Summary = GetString(doc, "fullName") ?? string.Empty,
                FileName = GetString(doc, "sourceId") ?? string.Empty,
                FileUrl = GetString(doc, "posted") ?? string.Empty,
                AppID = GetString(doc, "individualFlag") ?? string.Empty,
                ParentId = GetString(doc, "nameId") ?? string.Empty,
                Score = res.Score.HasValue ? res.Score.Value.ToString("0.00") : string.Empty,
                DocumentFields = new DocumentFields
                {
                    PolicyNumber = GetString(doc, "policyNumber") ?? string.Empty,
                    OrderNumber = GetString(doc, "orderNumber") ?? string.Empty,
                    PropertyAddress = GetString(doc, "propertyAddress") ?? string.Empty,
                    BorrowerName = GetString(doc, "borrowerName") ?? string.Empty,
                    SellerName = GetString(doc, "sellerName") ?? string.Empty,
                    BuyerName = GetString(doc, "buyerName") ?? string.Empty,
                    ClosingDate = GetString(doc, "closingDate") ?? string.Empty,
                    PolicyDate = GetString(doc, "policyDate") ?? string.Empty,
                }
            });
        }

        _logger.LogInformation("Search returned {count} results (query='{query}')", list.Count, request.Query);
        return list;
    }

    private static string? GetString(SearchDocument doc, string key)
        => doc.TryGetValue(key, out var val) ? val?.ToString() : null;

    private static SearchOptions BuildSearchOptions(UserQueryRequest request)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = request.PageSize <= 0 ? 10 : Math.Min(100, request.PageSize);

        var options = new SearchOptions
        {
            Size = pageSize,
            Skip = (page - 1) * pageSize,
            IncludeTotalCount = true,
            SearchMode = SearchMode.Any,
            QueryType = SearchQueryType.Full
        };

        // Default fields - customize as needed
        options.SearchFields.Add("fullName");
        options.OrderBy.Add("search.score() desc");
        return options;
    }
}
