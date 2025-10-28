using System.Collections.Generic;

namespace DocumentSummarizer.API.Models
{
    public class ResponseSummary
    {
        public string Summary { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string AppID { get; set; } = string.Empty;
        public string Parrent_Id { get; set; } = string.Empty;  // note: spelling per screenshot

        // Add new: Captions and Answer (optional)
        public List<string> Captions { get; set; } = new();
        public Dictionary<string, List<string>> Highlights { get; set; } = new();

        public DocumentFileds DocumentFileds { get; set; } = new();  // note: class name per screenshot

        public string Score { get; set; } = string.Empty;
        public string folderName { get; set; } = string.Empty;
    }

    public class SearchDocuments
    {
        public List<ResponseSummary> ResponseSummary { get; set; } = new();
        // public List<Answers> Answers { get; set; } = new(); // commented in screenshot
    }

    public class DocumentFileds
    {
        public string policyNumber { get; set; } = string.Empty;
        public string orderNumber { get; set; } = string.Empty;
        public string propertyAddress { get; set; } = string.Empty;
        public string borrowerName { get; set; } = string.Empty;
        public string sellerName { get; set; } = string.Empty;
        public string buyerName { get; set; } = string.Empty;
        public string closingDate { get; set; } = string.Empty;
        public string policyDate { get; set; } = string.Empty;
    }
}
