using System.Net;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Namesearch.Tests
{
    public class HealthzTests : IClassFixture<CustomFactory>
    {
        private readonly CustomFactory _factory;

        public HealthzTests(CustomFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Healthz_Returns_OK()
        {
            var client = _factory.CreateClient();
            var resp = await client.GetAsync("/healthz");
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            var body = await resp.Content.ReadAsStringAsync();
            Assert.Contains("OK", body);
        }
    }
}

public class CustomFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Avoid dev-time ValidateOnStart failures by using Production env in tests
        builder.UseEnvironment("Production");

        // Optionally inject minimal required configuration
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
    }
}
