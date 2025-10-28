using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Models;
using Xunit;

namespace Namesearch.Tests
{
    public class QueryEndpointTests : IClassFixture<QueryFactory>
    {
        private readonly QueryFactory _factory;
        public QueryEndpointTests(QueryFactory factory) { _factory = factory; }

        [Fact]
        public async Task Post_Query_Returns_200_With_Stubbed_Data()
        {
            var client = _factory.CreateClient();
            var payload = new UserQueryRequest
            {
                Query = "john",
                searchType = "semantic_hybrid",
                Page = 1,
                PageSize = 5
            };

            var resp = await client.PostAsJsonAsync("/api/rag/query", payload);
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

            var body = await resp.Content.ReadAsStringAsync();
            Assert.Contains("stub-summary", body);
            Assert.Contains("stub-file", body);
        }
    }
}

public class QueryFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            var dict = new Dictionary<string, string?>
            {
                ["AzureSearchServices:Endpoint"] = "https://example.search.windows.net",
                ["AzureSearchServices:ApiKey"] = "test-key",
                ["AzureSearchServices:IndexName"] = "hybrid"
            };
            cfg.AddInMemoryCollection(dict);
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<ISearchService>(_ => new StubSearchService());
        });
    }

    private class StubSearchService : ISearchService
    {
        public Task<List<ResponseSummary>> QueryDocumentAsync(UserQueryRequest request)
        {
            var list = new List<ResponseSummary>
            {
                new ResponseSummary { Summary = "stub-summary", FileName = "stub-file", FileUrl = "u" }
            };
            return Task.FromResult(list);
        }
    }
}
