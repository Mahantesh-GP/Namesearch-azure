using System.ComponentModel.DataAnnotations;

namespace DocumentSummarizer.API.Configurations
{
    /// <summary>
    /// Typed options for Azure Cognitive Search (flat binding).
    /// Does not validate on startup to avoid breaking startup in dev/prod without config.
    /// </summary>
    public class AzureSearchOptions
    {
        [Required]
        public string Endpoint { get; set; } = string.Empty;

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        [Required]
        public string IndexName { get; set; } = "hybrid";
    }
}
