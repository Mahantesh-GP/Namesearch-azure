using System.Collections.Generic;

namespace DocumentSummarizer.API.Models
{
    /// <summary>
    /// Represents a concise summary of search results returned to the client. It contains
    /// the summarised text and optionally identifiers of the documents that contributed to the summary.
    /// </summary>
    public class ResponseSummary
    {
        /// <summary>
        /// The summarised content returned to the user.
        /// </summary>
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// A collection of document identifiers or names that were used to construct the summary.
        /// </summary>
        public List<string> SourceDocuments { get; set; } = new List<string>();
    }
}