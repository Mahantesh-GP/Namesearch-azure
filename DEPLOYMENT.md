# Namesearch Application Deployment Guide

## Overview
This application combines a Blazor Server UI and Web API into a single ASP.NET Core application that can be deployed to Azure App Service.

## Architecture
- **Frontend**: Blazor Server with interactive components
- **Backend**: ASP.NET Core Minimal API
- **Deployment**: Single Azure App Service (Linux)

## Local Development

### Prerequisites
- .NET 8.0 SDK
- Azure subscription (for deployment)
- Azure AI Search service

### Running Locally

1. Update `appsettings.Development.json` in `Namesearch.Api` with your Azure Search configuration:
```json
{
  "AzureSearch": {
    "ServiceName": "your-search-service",
    "Endpoint": "https://your-search-service.search.windows.net",
    "IndexName": "your-index-name",
    "ApiKey": "your-api-key"
  }
}
```

2. Build the solution:
```powershell
dotnet build
```

3. Run the application:
```powershell
cd src/Namesearch.Api
dotnet run
```

4. Open your browser to `https://localhost:7000` (or the port shown in the console)

## Azure Deployment

### Option 1: Deploy using Azure CLI

1. **Update parameters file**:
   Edit `infra/main.parameters.json` with your Azure Search details.

2. **Create a resource group**:
```powershell
az group create --name rg-namesearch --location eastus
```

3. **Deploy infrastructure**:
```powershell
az deployment group create `
  --resource-group rg-namesearch `
  --template-file infra/main.bicep `
  --parameters infra/main.parameters.json
```

4. **Publish the application**:
```powershell
# From the solution root
dotnet publish src/Namesearch.Api/Namesearch.Api.csproj -c Release -o ./publish

# Create a zip file
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force

# Deploy to App Service
az webapp deployment source config-zip `
  --resource-group rg-namesearch `
  --name <app-service-name> `
  --src ./app.zip
```

### Option 2: Deploy using Visual Studio

1. Right-click on `Namesearch.Api` project
2. Select **Publish**
3. Choose **Azure** → **Azure App Service (Linux)**
4. Select your subscription and create/select an App Service
5. Configure the App Service settings with your Azure Search details
6. Click **Publish**

### Option 3: Deploy using VS Code

1. Install the Azure App Service extension
2. Right-click on `Namesearch.Api` folder
3. Select **Deploy to Web App**
4. Follow the prompts

## Configuration

### Required App Settings (Azure)
- `AzureSearch__ServiceName`: Your Azure Search service name
- `AzureSearch__Endpoint`: Your Azure Search endpoint
- `AzureSearch__IndexName`: Your search index name
- `AzureSearch__ApiKey`: Your Azure Search API key (or use Managed Identity)
- `ApiBaseUrl`: The base URL of your deployed app (e.g., https://your-app.azurewebsites.net)

### Using Managed Identity (Recommended)
Instead of using an API key, configure Managed Identity:

1. The Bicep template already enables System-Assigned Managed Identity
2. Grant the App Service's identity the "Search Index Data Reader" role on your Azure Search service:
```powershell
az role assignment create `
  --role "Search Index Data Reader" `
  --assignee <principal-id-from-deployment-output> `
  --scope /subscriptions/<subscription-id>/resourceGroups/<resource-group>/providers/Microsoft.Search/searchServices/<search-service-name>
```

3. Update your configuration to use DefaultAzureCredential (already implemented in the code)

## Project Structure
```
src/
├── Namesearch.Api/          # Main application (hosts both UI and API)
├── Namesearch.Web/          # Blazor UI components and pages
├── Namesearch.Application/  # Business logic (CQRS handlers)
├── Namesearch.Infrastructure/ # External services (Azure Search)
└── Namesearch.Domain/       # Domain models

infra/
├── main.bicep              # Infrastructure as Code
└── main.parameters.json    # Deployment parameters
```

## Features
- Modern, responsive search UI
- Real-time search with filters (County, Individual Flag)
- Server-side rendering with Blazor Server
- Clean architecture with separation of concerns
- Reusable component design
- CQRS pattern with MediatR
- FluentValidation for request validation
- Health checks endpoint

## Troubleshooting

### UI not loading
- Ensure both `Namesearch.Api` and `Namesearch.Web` projects are built
- Check that the `ApiBaseUrl` configuration is correct
- Verify CORS settings in production

### Search not working
- Verify Azure Search credentials
- Check that the search index exists and has data
- Review application logs in Azure Portal

### Build errors
- Run `dotnet restore` from the solution root
- Ensure all project references are correct
- Check that .NET 8.0 SDK is installed
