# Namesearch - Modern Name Search Application

[![.NET Build & Test](https://github.com/Mahantesh-GP/Namesearch-azure/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Mahantesh-GP/Namesearch-azure/actions/workflows/dotnet.yml)

A modern, full-stack name search application built with Clean Architecture, featuring a Blazor Server UI and ASP.NET Core Web API, designed for deployment to Azure App Service.

## Features

- ✨ Modern, responsive Blazor UI with real-time search
- 🤖 **AI-Powered Nickname Variations** - Uses Azure OpenAI to generate nickname variations for enhanced search results
- 🔍 Advanced filtering (County, Individual Flag)
- 🏗️ Clean Architecture with SOLID principles
- 🎯 CQRS pattern with MediatR
- ✅ FluentValidation for request validation
- 🔐 Azure Search integration with Managed Identity support
- 📦 Single deployment package (UI + API)
- 🏥 Health checks and monitoring
- 🎨 Reusable component design

## Architecture

### Project Structure

```
src/
├── Namesearch.Api/          # Main application - hosts both UI and API
├── Namesearch.Web/          # Blazor Server UI components and pages
├── Namesearch.Application/  # Business logic (CQRS handlers, validators)
├── Namesearch.Infrastructure/ # External services (Azure Search)
└── Namesearch.Domain/       # Domain models and primitives

tests/
└── Namesearch.UnitTests/    # Unit tests

infra/
├── main.bicep              # Infrastructure as Code (Azure resources)
└── main.parameters.json    # Deployment parameters
```

### Design Principles

- **Clean Architecture**: Clear separation between domain, application, infrastructure layers
- **CQRS**: Command Query Responsibility Segregation with MediatR
- **Dependency Injection**: All dependencies injected via ASP.NET Core DI
- **Component-Based UI**: Reusable Blazor components following single responsibility
- **Infrastructure as Code**: Bicep templates for reproducible deployments

## Quick Start

### Prerequisites
- .NET 8 SDK
- Azure subscription (for deployment)
- Azure AI Search service

### Local Development

1. **Clone the repository**
```powershell
git clone https://github.com/Mahantesh-GP/Namesearch-azure.git
cd NamesearchTemplate
```

2. **Configure Azure Search and OpenAI**
   
   Update `src/Namesearch.Api/appsettings.Development.json`:
```json
{
  "AzureSearch": {
    "ServiceName": "your-search-service",
    "Endpoint": "https://your-search-service.search.windows.net",
    "IndexName": "your-index-name",
    "ApiKey": "your-api-key"
  },
  "OpenAI": {
    "Endpoint": "https://your-openai-service.openai.azure.com",
    "ApiKey": "your-openai-api-key",
    "DeploymentName": "gpt-4",
    "MaxTokens": 150,
    "Temperature": 0.3
  }
}
```

3. **Build the solution**
```powershell
dotnet build
```

4. **Run the application**
```powershell
dotnet run --project src/Namesearch.Api/Namesearch.Api.csproj
```

5. **Open in browser**
   - UI: https://localhost:7000 (or the port shown in console)
   - API Swagger: https://localhost:7000/swagger
   - Health Check: https://localhost:7000/healthz

## UI Components

The application includes three main reusable components:

### SearchBar
- Real-time search input with loading state
- Enter key support
- Disabled state during search

### FilterDropdown
- Generic, type-safe dropdown component
- Supports string and bool? types
- Custom styling with icons

### SearchResults
- Card-based result display
- Pagination support
- Document field highlighting
- Loading and empty states

See [Web UI README](src/Namesearch.Web/README.md) for detailed component documentation.

## AI-Powered Nickname Variations

The application uses Azure OpenAI to enhance search results by automatically generating nickname variations:

**How it works:**
1. User enters a name (e.g., "Jonathan")
2. OpenAI generates nickname variations (e.g., ["Jonathan", "John", "Johnny", "Jon"])
3. Search query is enriched with all variations: `"Jonathan" OR "John" OR "Johnny" OR "Jon"`
4. Azure Search returns results matching any variation

**UI Display:**
- Nickname variations are displayed in a beautiful gradient card above search results
- Users can see exactly which variations were used to enhance their search
- Improves transparency and understanding of AI-assisted search

**Benefits:**
- Improved search recall - find documents even if they use different name variations
- No manual nickname configuration needed
- Automatically handles cultural variations and common nicknames

**Example:**
- Search for "Bob" → Also finds "Robert", "Bobby", "Roberto"
- Search for "Liz" → Also finds "Elizabeth", "Beth", "Lizzie"

## API Endpoints

### POST /api/search
Search for names in the index.

**Request:**
```json
{
  "query": "John Smith",
  "selectedAppId": "",
  "searchType": "semantic_hybrid",
  "page": 1,
  "pageSize": 10
}
```

**Response:**
```json
[
  {
    "summary": "Document summary...",
    "fileName": "doc123.pdf",
    "documentFields": {
      "borrowerName": "John Smith",
      "propertyAddress": "123 Main St, Miami, FL",
      "policyNumber": "POL-12345",
      "closingDate": "2024-01-15"
    },
    "score": "0.95",
    "captions": ["...relevant text..."]
  }
]
```

### GET /healthz
Health check endpoint for monitoring.

## Deployment to Azure

See [DEPLOYMENT.md](DEPLOYMENT.md) for detailed deployment instructions.

### Quick Deploy

1. **Update parameters**
```powershell
# Edit infra/main.parameters.json with your Azure Search details
```

2. **Create resource group**
```powershell
az group create --name rg-namesearch --location eastus
```

3. **Deploy infrastructure**
```powershell
az deployment group create `
  --resource-group rg-namesearch `
  --template-file infra/main.bicep `
  --parameters infra/main.parameters.json
```

4. **Publish application**
```powershell
dotnet publish src/Namesearch.Api/Namesearch.Api.csproj -c Release -o ./publish
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force

az webapp deployment source config-zip `
  --resource-group rg-namesearch `
  --name <app-service-name> `
  --src ./app.zip
```

## Configuration

### Required Settings

- `AzureSearch__ServiceName`: Azure Search service name
- `AzureSearch__Endpoint`: Azure Search endpoint URL
- `AzureSearch__IndexName`: Search index name
- `AzureSearch__ApiKey`: API key (or use Managed Identity)
- `ApiBaseUrl`: Application base URL

### Managed Identity (Recommended)

The Bicep template automatically enables System-Assigned Managed Identity. Grant the identity "Search Index Data Reader" role:

```powershell
az role assignment create `
  --role "Search Index Data Reader" `
  --assignee <principal-id> `
  --scope <search-service-resource-id>
```

## Testing

```powershell
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Middleware Features

- **Health Checks**: `/healthz` for readiness/liveness probes
- **CORS**: Configured for Blazor SignalR support
- **Problem Details**: Standardized error responses
- **Exception Handling**: Global exception handling middleware
- **Static Files**: Serves Blazor UI assets
- **Antiforgery**: Protection for interactive Blazor components

## Technology Stack

- **Backend**: ASP.NET Core 8.0 (Minimal API)
- **Frontend**: Blazor Server
- **CQRS**: MediatR
- **Validation**: FluentValidation
- **Search**: Azure AI Search
- **Infrastructure**: Azure App Service (Linux)
- **IaC**: Bicep
- **Testing**: xUnit, Moq, FluentAssertions

## Security Best Practices

✅ HTTPS enforced  
✅ Managed Identity for Azure services  
✅ API key storage in configuration (not in code)  
✅ CORS properly configured  
✅ Antiforgery tokens for Blazor  
✅ Input validation with FluentValidation  

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add/update tests
5. Submit a pull request

## Troubleshooting

### UI not loading
- Verify both projects are built
- Check `ApiBaseUrl` configuration
- Review browser console for errors

### Search not working
- Verify Azure Search credentials
- Check search index exists
- Review application logs

### Build errors
- Run `dotnet restore`
- Ensure .NET 8.0 SDK is installed
- Check all project references

## Next Steps

- [ ] Add authentication/authorization
- [ ] Implement advanced search filters
- [ ] Add export functionality
- [ ] Dark mode support
- [ ] Progressive Web App (PWA)
- [ ] Integration tests with WebApplicationFactory
- [ ] OpenTelemetry for distributed tracing
- [ ] CI/CD pipeline with GitHub Actions

## License

This project is licensed under the MIT License.

## Support

For issues and questions:
- Create an issue in this repository
- Check existing documentation
- Review Azure Search documentation
