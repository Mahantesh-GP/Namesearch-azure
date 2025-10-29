using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Namesearch.Application.Abstractions;
using Namesearch.Infrastructure.Configuration;
using OpenAI.Chat;

namespace Namesearch.Infrastructure.AI;

public sealed class OpenAIService : IOpenAIService
{
    private readonly AzureOpenAIClient _client;
    private readonly OpenAIOptions _options;
    private readonly ILogger<OpenAIService> _logger;

    public OpenAIService(IOptions<OpenAIOptions> options, ILogger<OpenAIService> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (string.IsNullOrWhiteSpace(_options.Endpoint) ||
            string.IsNullOrWhiteSpace(_options.ApiKey) ||
            string.IsNullOrWhiteSpace(_options.DeploymentName))
        {
            throw new InvalidOperationException("OpenAIOptions must include Endpoint, ApiKey, and DeploymentName.");
        }

        _client = new AzureOpenAIClient(new Uri(_options.Endpoint), new AzureKeyCredential(_options.ApiKey));
    }

    public async Task<IReadOnlyList<string>> GetNicknameVariationsAsync(string name, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new List<string> { name }.AsReadOnly();
        }

        try
        {
            _logger.LogInformation("Generating nickname variations for: {Name}", name);

            var chatClient = _client.GetChatClient(_options.DeploymentName);

            var systemPrompt = @"You are a helpful assistant that generates nickname variations for names. 
Given a name, provide common nicknames, shortened versions, and alternative spellings.
Return only the variations as a JSON array of strings, without any explanation.
Include the original name in the list.
Example: For 'Jonathan', return [""Jonathan"", ""John"", ""Johnny"", ""Jon"", ""Johnathan""]";

            var userPrompt = $"Generate nickname variations for: {name}";

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(userPrompt)
            };

            var chatCompletionsOptions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = _options.MaxTokens,
                Temperature = _options.Temperature
            };

            var response = await chatClient.CompleteChatAsync(messages, chatCompletionsOptions, cancellationToken);

            var content = response.Value.Content[0].Text;
            
            _logger.LogDebug("OpenAI response: {Response}", content);

            // Parse the JSON array from the response
            var variations = ParseNicknameVariations(content, name);

            _logger.LogInformation("Generated {Count} nickname variations for {Name}", variations.Count, name);

            return variations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating nickname variations for {Name}. Returning original name only.", name);
            // Fallback: return just the original name if OpenAI call fails
            return new List<string> { name }.AsReadOnly();
        }
    }

    private IReadOnlyList<string> ParseNicknameVariations(string content, string originalName)
    {
        try
        {
            // Try to extract JSON array from the response
            var startIndex = content.IndexOf('[');
            var endIndex = content.LastIndexOf(']');

            if (startIndex >= 0 && endIndex > startIndex)
            {
                var jsonArray = content.Substring(startIndex, endIndex - startIndex + 1);
                var variations = JsonSerializer.Deserialize<List<string>>(jsonArray);

                if (variations != null && variations.Any())
                {
                    // Ensure original name is included
                    if (!variations.Contains(originalName, StringComparer.OrdinalIgnoreCase))
                    {
                        variations.Insert(0, originalName);
                    }

                    // Remove duplicates (case-insensitive) and empty strings
                    var distinctVariations = variations
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    return distinctVariations.AsReadOnly();
                }
            }

            // If parsing fails, try splitting by common delimiters
            var fallbackVariations = content
                .Replace("[", "")
                .Replace("]", "")
                .Replace("\"", "")
                .Split(new[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (fallbackVariations.Any())
            {
                if (!fallbackVariations.Contains(originalName, StringComparer.OrdinalIgnoreCase))
                {
                    fallbackVariations.Insert(0, originalName);
                }
                return fallbackVariations.AsReadOnly();
            }
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse OpenAI response as JSON. Content: {Content}", content);
        }

        // Ultimate fallback: return just the original name
        return new List<string> { originalName }.AsReadOnly();
    }
}
