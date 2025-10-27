namespace DocumentSummarizer.API.Enum
{
    /// <summary>
    /// Represents the strategy used when performing a search. Different strategies may
    /// produce different results or operate with distinct ranking models.
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// A standard keyword search.
        /// </summary>
        Keyword,

        /// <summary>
        /// A semantic search using machine learning models to interpret meaning.
        /// </summary>
        Semantic
    }
}