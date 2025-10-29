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
        // Arrange
        var searchService = Substitute.For<IAzureSearchService>();
        var openAIService = Substitute.For<IOpenAIService>();
        
        var expected = new List<ResponseSummary> { new ResponseSummary { Summary = "John Doe" } } as IReadOnlyList<ResponseSummary>;
        var nicknameVariations = new List<string> { "john", "Johnny", "Jon" } as IReadOnlyList<string>;
        
        openAIService.GetNicknameVariationsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(nicknameVariations);
        
        searchService.SearchAsync(Arg.Any<UserQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var handler = new QueryDocuments.Handler(searchService, openAIService);
        var result = await handler.Handle(new QueryDocuments.Query(new UserQueryRequest { Query = "john" }), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expected);
        await openAIService.Received(1).GetNicknameVariationsAsync("john", Arg.Any<CancellationToken>());
        await searchService.Received(1).SearchAsync(Arg.Any<UserQueryRequest>(), Arg.Any<CancellationToken>());
    }
}
