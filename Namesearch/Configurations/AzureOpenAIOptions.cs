using System.ComponentModel.DataAnnotations;

namespace DocumentSummarizer.API.Configurations
{
    /// <summary>
    /// Options class binding to the "AzureOpenAI" section of appsettings.json. When bound,
    /// these properties configure how the application connects to Azure OpenAI or a similar service.
    /// </summary>
    public class AzureOpenAIOptions
    {
        /// <summary>
        /// The base endpoint of the OpenAI service (e.g. https://your-openai-endpoint.azure.com/).
        /// </summary>
        [Url]
        public string Endpoint { get; set; } = string.Empty;

        /// <summary>
        /// The API key used for authenticating requests.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// The name of the specific deployment or model to target within the service.
        /// </summary>
        public string DeploymentName { get; set; } = string.Empty;
    }
}