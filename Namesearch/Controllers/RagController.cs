using System.Threading.Tasks;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocumentSummarizer.API.Controllers
{
    /// <summary>
    /// A simple REST controller that exposes endpoints for retrieving summarised search results.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RagController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public RagController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        /// <summary>
        /// Summarises documents relevant to the provided query.
        /// </summary>
        /// <param name="request">The user's search and summarisation request.</param>
        /// <returns>A summary of the relevant documents.</returns>
        [HttpPost("summarize")]
        public async Task<ActionResult<ResponseSummary>> Summarize([FromBody] UserQueryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest("Query cannot be empty.");
            }

            var result = await _searchService.SummariseAsync(request);
            return Ok(result);
        }
    }
}