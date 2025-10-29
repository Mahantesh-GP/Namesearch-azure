using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Namesearch.Application.Abstractions;
using Namesearch.Infrastructure.AI;
using Namesearch.Infrastructure.Configuration;
using Namesearch.Infrastructure.Search;

namespace Namesearch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Azure Search configuration
        services.AddOptions<AzureSearchOptions>()
            .Bind(configuration.GetSection("AzureSearchServices"))
            .ValidateDataAnnotations();

        services.AddSingleton<IAzureSearchService, AzureSearchService>();

        // OpenAI configuration
        services.AddOptions<OpenAIOptions>()
            .Bind(configuration.GetSection("OpenAI"))
            .ValidateDataAnnotations();

        services.AddSingleton<IOpenAIService, OpenAIService>();

        return services;
    }
}
