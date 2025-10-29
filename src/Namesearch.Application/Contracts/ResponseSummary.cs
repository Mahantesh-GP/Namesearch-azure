using System.Collections.Generic;

namespace Namesearch.Application.Contracts;

public sealed class ResponseSummary
{
    public string Summary { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string AppID { get; init; } = string.Empty;
    public string ParentId { get; init; } = string.Empty;

    public IReadOnlyList<string> Captions { get; init; } = new List<string>();
    public IReadOnlyDictionary<string, IReadOnlyList<string>> Highlights { get; init; } = new Dictionary<string, IReadOnlyList<string>>();

    public DocumentFields DocumentFields { get; init; } = new();

    public string Score { get; init; } = string.Empty;
    public string FolderName { get; init; } = string.Empty;
}

public sealed class DocumentFields
{
    public string PolicyNumber { get; init; } = string.Empty;
    public string OrderNumber { get; init; } = string.Empty;
    public string PropertyAddress { get; init; } = string.Empty;
    public string BorrowerName { get; init; } = string.Empty;
    public string SellerName { get; init; } = string.Empty;
    public string BuyerName { get; init; } = string.Empty;
    public string ClosingDate { get; init; } = string.Empty;
    public string PolicyDate { get; init; } = string.Empty;
}
