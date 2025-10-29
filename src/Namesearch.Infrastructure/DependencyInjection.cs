using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Namesearch.Application.Abstractions;
using Namesearch.Infrastructure.Configuration;
using Namesearch.Infrastructure.Search;

namespace Namesearch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AzureSearchOptions>()
            .Bind(configuration.GetSection("AzureSearchServices"))
            .ValidateDataAnnotations();

        services.AddSingleton<IAzureSearchService, AzureSearchService>();
        return services;
    }
}
