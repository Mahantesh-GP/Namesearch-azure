using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Models;
using DocumentSummarizer.API.Services;

namespace DocumentSummarizer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RagController : ControllerBase
    {
        private readonly IQueryDocumentService _queryService;
        private readonly ISearchService _service;
        private readonly ILogger<RagController> _logger;
    // Removed unused concrete service to keep DI minimal

        public RagController(
            IQueryDocumentService queryService,
            ISearchService service,
            ILogger<RagController> logger)
        {
            _queryService = queryService;
            _service = service;
            _logger = logger;
        }

        [HttpPost("query")]
        public async Task<IActionResult> QueryDocument([FromBody] UserQueryRequest request)
        {
            _logger.LogInformation("QueryDocument started with query: {query}", request.Query);

            if (request == null || string.IsNullOrEmpty(request.Query))
            {
                _logger.LogWarning("Query cannot be empty");
                return BadRequest("Query cannot be empty");
            }

            try
            {
                var result = await _service.QueryDocumentAsync(request);
                _logger.LogInformation("QueryDocument completed successfully for query: {query}", request.Query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying document for query: {query}", request.Query);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
