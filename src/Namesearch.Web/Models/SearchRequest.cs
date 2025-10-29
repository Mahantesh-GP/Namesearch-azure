namespace Namesearch.Web.Models;

public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public string? County { get; set; }
    public bool? IndividualFlag { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
