using System.Threading.Tasks;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Models;

namespace DocumentSummarizer.API.Services
{
    /// <summary>
    /// Stub implementation of <see cref="IQueryDocumentService"/>. A real implementation would
    /// call a language model such as Azure OpenAI to summarise the retrieved documents.
    /// </summary>
    public class QueryDocumentService : IQueryDocumentService
    {
        public Task<ResponseSummary> GetDocumentSummaryAsync(UserQueryRequest request)
        {
            // Return a placeholder summary. Replace with calls to a language model in production.
            var summary = new ResponseSummary
            {
                Summary = $"This is a stubbed summary for query '{request.Query}'."
            };
            return Task.FromResult(summary);
        }
    }
}