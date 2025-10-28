namespace DocumentSummarizer.API.Models
{
    public class UserQueryRequest
    {
        public string Query { get; set; } = string.Empty;
        public string SelectedAppId { get; set; } = string.Empty;

        // Per screenshot: searchType is a string with default "vector"
        public string searchType { get; set; } = "vector";

        public string SelectedDateField { get; set; } = string.Empty;
        public string SelectedSearchFiled { get; set; } = string.Empty;
        public string SelectedDuration { get; set; } = string.Empty;

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
