namespace DocumentSummarizer.API.Enum
{
    /// <summary>
    /// Identifies the context within which a search should be executed. These values can be
    /// extended to tune search behaviour across different fields or scopes.
    /// </summary>
    public enum SearchContext
    {
        /// <summary>
        /// Search across all available fields.
        /// </summary>
        All,

        /// <summary>
        /// Restrict the search to document titles only.
        /// </summary>
        Title,

        /// <summary>
        /// Restrict the search to document content only.
        /// </summary>
        Content
    }
}