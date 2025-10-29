using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Namesearch.Application.Abstractions;

/// <summary>
/// Service for generating nickname variations using OpenAI
/// </summary>
public interface IOpenAIService
{
    /// <summary>
    /// Generates nickname variations for a given name to enhance search results
    /// </summary>
    /// <param name="name">The original name to generate variations for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of nickname variations including the original name</returns>
    Task<IReadOnlyList<string>> GetNicknameVariationsAsync(string name, CancellationToken cancellationToken = default);
}
