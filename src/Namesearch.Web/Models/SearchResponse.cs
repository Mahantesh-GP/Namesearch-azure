namespace Namesearch.Web.Models;

public class SearchResponse
{
    public List<SearchResult> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class SearchResult
{
    public string Summary { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string AppID { get; set; } = string.Empty;
    public string ParentId { get; set; } = string.Empty;
    public List<string> Captions { get; set; } = new();
    public Dictionary<string, List<string>> Highlights { get; set; } = new();
    public DocumentFields DocumentFields { get; set; } = new();
    public string Score { get; set; } = string.Empty;
    public string FolderName { get; set; } = string.Empty;
}

public class DocumentFields
{
    public string PolicyNumber { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string BorrowerName { get; set; } = string.Empty;
    public string SellerName { get; set; } = string.Empty;
    public string BuyerName { get; set; } = string.Empty;
    public string ClosingDate { get; set; } = string.Empty;
    public string PolicyDate { get; set; } = string.Empty;
}
