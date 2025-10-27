using System.Threading.Tasks;
using DocumentSummarizer.API.Models;

namespace DocumentSummarizer.API.Interfaces
{
    /// <summary>
    /// Handles querying of documents and producing summarised responses. This service might wrap calls
    /// to external language models such as OpenAI.
    /// </summary>
    public interface IQueryDocumentService
    {
        /// <summary>
        /// Returns a concise summary of the documents relevant to the given request.
        /// </summary>
        /// <param name="request">The request describing what the user would like to know.</param>
        /// <returns>A <see cref="ResponseSummary"/> containing the summarised response.</returns>
        Task<ResponseSummary> GetDocumentSummaryAsync(UserQueryRequest request);
    }
}