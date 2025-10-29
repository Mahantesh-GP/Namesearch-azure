using System.ComponentModel.DataAnnotations;

namespace Namesearch.Infrastructure.Configuration;

public sealed class AzureSearchOptions
{
    [Required]
    public string Endpoint { get; init; } = string.Empty;

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string IndexName { get; init; } = string.Empty;
}
