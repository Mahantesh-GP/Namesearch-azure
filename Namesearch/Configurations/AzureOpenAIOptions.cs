namespace DocumentSummarizer.API.Configurations
{
    public class AzureOpenAIOptions
    {
        public string DeploymentName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string EmbeddingDeploymentName { get; set; } = string.Empty;
    }
}
