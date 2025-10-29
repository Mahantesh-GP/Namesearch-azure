using System.Collections.Generic;
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

        public Handler(IAzureSearchService search) => _search = search;

        public Task<IReadOnlyList<ResponseSummary>> Handle(Query request, CancellationToken cancellationToken)
        {
            return _search.SearchAsync(request.Request, cancellationToken);
        }
    }
}
