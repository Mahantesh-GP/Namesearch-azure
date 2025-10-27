using System;
using DocumentSummarizer.API.Enum;

namespace DocumentSummarizer.API.Models
{
    /// <summary>
    /// Encapsulates a request from the user containing a query string and optional parameters
    /// controlling how the search and summarisation should behave.
    /// </summary>
    public class UserQueryRequest
    {
        /// <summary>
        /// The free‑text query submitted by the user.
        /// </summary>
        public string Query { get; set; } = string.Empty;

        /// <summary>
        /// A natural language description of a date filter (e.g., "last week").
        /// This can be parsed by <see cref="Functions.DateFilterParser"/>.
        /// </summary>
        public string? DateFilter { get; set; }
        
        /// <summary>
        /// Specifies the context in which the search should be performed. Defaults to all fields.
        /// </summary>
        public SearchContext SearchContext { get; set; } = SearchContext.All;

        /// <summary>
        /// Specifies the type of search to perform. Defaults to keyword search.
        /// </summary>
        public SearchType SearchType { get; set; } = SearchType.Keyword;
    }
}