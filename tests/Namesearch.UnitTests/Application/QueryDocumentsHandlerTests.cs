using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Namesearch.Application.Abstractions;
using Namesearch.Application.Contracts;
using Namesearch.Application.Features.Search;
using NSubstitute;
using Xunit;

namespace Namesearch.UnitTests.Application;

public class QueryDocumentsHandlerTests
{
    [Fact]
    public async Task Returns_results_from_search_service()
    {
        var stub = Substitute.For<IAzureSearchService>();
        var expected = new List<ResponseSummary> { new ResponseSummary { Summary = "John Doe" } } as IReadOnlyList<ResponseSummary>;
        stub.SearchAsync(Arg.Any<UserQueryRequest>(), Arg.Any<CancellationToken>()).Returns(expected);

        var handler = new QueryDocuments.Handler(stub);
        var result = await handler.Handle(new QueryDocuments.Query(new UserQueryRequest { Query = "john" }), CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }
}
