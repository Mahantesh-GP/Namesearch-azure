using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Namesearch.Application.Abstractions;
using Namesearch.Application.Contracts;

namespace Namesearch.Application.Features.Search;

public static class QueryDocuments
{
    // Contract
    public sealed record Query(UserQueryRequest Request) : IRequest<IReadOnlyList<ResponseSummary>>;

    // Handler
    public sealed class Handler : IRequestHandler<Query, IReadOnlyList<ResponseSummary>>
    {
        private readonly IAzureSearchService _search;
        private readonly IOpenAIService _openAI;

        public Handler(IAzureSearchService search, IOpenAIService openAI)
        {
            _search = search;
            _openAI = openAI;
        }

        public async Task<IReadOnlyList<ResponseSummary>> Handle(Query request, CancellationToken cancellationToken)
        {
            // Step 1: Get nickname variations using OpenAI
            var nicknameVariations = await _openAI.GetNicknameVariationsAsync(request.Request.Query, cancellationToken);

            // Step 2: Create an enriched query with all variations
            var enrichedQuery = string.Join(" OR ", nicknameVariations.Select(n => $"\"{n}\""));

            // Step 3: Create a modified request with the enriched query
            var enrichedRequest = new UserQueryRequest
            {
                Query = enrichedQuery,
                SelectedAppId = request.Request.SelectedAppId,
                SearchType = request.Request.SearchType,
                SelectedDateField = request.Request.SelectedDateField,
                SelectedSearchField = request.Request.SelectedSearchField,
                SelectedDuration = request.Request.SelectedDuration,
                Page = request.Request.Page,
                PageSize = request.Request.PageSize
            };

            // Step 4: Execute the search with enriched query
            return await _search.SearchAsync(enrichedRequest, cancellationToken);
        }
    }
}
