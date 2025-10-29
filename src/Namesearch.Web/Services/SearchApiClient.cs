using System.Net.Http.Json;
using Namesearch.Web.Models;

namespace Namesearch.Web.Services;

public class SearchApiClient : ISearchApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SearchApiClient> _logger;

    public SearchApiClient(HttpClient httpClient, ILogger<SearchApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Map the UI request to the backend API request format
            var apiRequest = new
            {
                query = request.Query,
                selectedAppId = string.Empty,
                searchType = "semantic_hybrid",
                selectedDateField = string.Empty,
                selectedSearchField = string.Empty,
                selectedDuration = string.Empty,
                page = request.Page,
                pageSize = request.PageSize
            };

            var response = await _httpClient.PostAsJsonAsync("/api/search", apiRequest, cancellationToken);
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiSearchResponse>(cancellationToken);
            
            if (apiResponse == null)
            {
                return new SearchResponse();
            }

            var results = apiResponse.Results ?? new List<SearchResult>();

            // Filter by county and individual flag if specified
            if (!string.IsNullOrEmpty(request.County))
            {
                results = results.Where(r => r.DocumentFields?.PropertyAddress?.Contains(request.County, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
            }

            if (request.IndividualFlag.HasValue)
            {
                // Assuming individual flag is true when BorrowerName is present
                results = results.Where(r => request.IndividualFlag.Value ? 
                    !string.IsNullOrEmpty(r.DocumentFields?.BorrowerName) : 
                    string.IsNullOrEmpty(r.DocumentFields?.BorrowerName)).ToList();
            }

            return new SearchResponse
            {
                Results = results,
                NicknameVariations = apiResponse.NicknameVariations ?? new List<string>(),
                OriginalQuery = apiResponse.OriginalQuery ?? request.Query,
                TotalCount = results.Count,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching documents");
            return new SearchResponse();
        }
    }
}

// DTO for API response
internal class ApiSearchResponse
{
    public List<SearchResult>? Results { get; set; }
    public List<string>? NicknameVariations { get; set; }
    public string? OriginalQuery { get; set; }
}
