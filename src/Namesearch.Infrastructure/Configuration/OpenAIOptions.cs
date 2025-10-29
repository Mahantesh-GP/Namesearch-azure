using System.ComponentModel.DataAnnotations;

namespace Namesearch.Infrastructure.Configuration;

public sealed class OpenAIOptions
{
    [Required]
    public string Endpoint { get; init; } = string.Empty;

    [Required]
    public string ApiKey { get; init; } = string.Empty;

    [Required]
    public string DeploymentName { get; init; } = string.Empty;
    
    public int MaxTokens { get; init; } = 150;
    
    public float Temperature { get; init; } = 0.7f;
}
