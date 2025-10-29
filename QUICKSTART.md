# Quick Start Guide - Namesearch Application

## 🚀 Run Locally in 3 Steps

### Step 1: Configure Azure Search and OpenAI
Edit `src/Namesearch.Api/appsettings.Development.json`:
```json
{
  "AzureSearch": {
    "ServiceName": "your-service-name",
    "Endpoint": "https://your-service.search.windows.net",
    "IndexName": "your-index",
    "ApiKey": "your-key"
  },
  "OpenAI": {
    "Endpoint": "https://your-openai.openai.azure.com",
    "ApiKey": "your-openai-key",
    "DeploymentName": "gpt-4",
    "MaxTokens": 150,
    "Temperature": 0.3
  }
}
```

### Step 2: Build & Run
```powershell
dotnet build
dotnet run --project src/Namesearch.Api/Namesearch.Api.csproj
```

### Step 3: Open Browser
Navigate to: `https://localhost:7000`

---

## 🌐 Deploy to Azure in 4 Steps

### Step 1: Update Parameters
Edit `infra/main.parameters.json`:
```json
{
  "parameters": {
    "searchServiceName": { "value": "your-search-service" },
    "searchServiceEndpoint": { "value": "https://your-search.search.windows.net" },
    "searchIndexName": { "value": "your-index" },
    "openAIEndpoint": { "value": "https://your-openai.openai.azure.com" },
    "openAIApiKey": { "value": "your-openai-key" },
    "openAIDeploymentName": { "value": "gpt-4" }
  }
}
```

### Step 2: Create Resource Group
```powershell
az group create --name rg-namesearch --location eastus
```

### Step 3: Deploy Infrastructure
```powershell
az deployment group create `
  --resource-group rg-namesearch `
  --template-file infra/main.bicep `
  --parameters infra/main.parameters.json
```

### Step 4: Publish Application
```powershell
# Publish
dotnet publish src/Namesearch.Api/Namesearch.Api.csproj -c Release -o ./publish

# Package
Compress-Archive -Path ./publish/* -DestinationPath ./app.zip -Force

# Deploy (replace <app-name> with the output from step 3)
az webapp deployment source config-zip `
  --resource-group rg-namesearch `
  --name <app-name> `
  --src ./app.zip
```

**Your app is now live at: `https://<app-name>.azurewebsites.net`**

---

## 📁 Project Overview

```
Namesearch Solution
│
├── 🎨 Namesearch.Web         → Blazor UI (components, pages, CSS)
├── 🔌 Namesearch.Api         → Web API + hosts Blazor UI
├── 💼 Namesearch.Application → Business logic (CQRS)
├── 🏗️ Namesearch.Infrastructure → Azure Search integration
├── 🎯 Namesearch.Domain      → Domain models
└── 🧪 Namesearch.UnitTests   → Unit tests
```

---

## 🔑 Key URLs (Local)

| Service | URL |
|---------|-----|
| **UI** | https://localhost:7000 |
| **API Swagger** | https://localhost:7000/swagger |
| **Health Check** | https://localhost:7000/healthz |

---

## 🎨 UI Components

| Component | Purpose |
|-----------|---------|
| **SearchBar** | Search input with loading state |
| **FilterDropdown** | Generic filter component |
| **SearchResults** | Results display with pagination |

---

## 🔍 Available Filters

1. **County** - Filter by property county
   - Miami-Dade
   - Broward
   - Palm Beach
   - Orange
   - Hillsborough

2. **Individual Flag** - Filter by type
   - Individual (has borrower name)
   - Business (no borrower name)

---

## 🛠️ Common Commands

```powershell
# Build
dotnet build

# Run tests
dotnet test

# Run locally
dotnet run --project src/Namesearch.Api/Namesearch.Api.csproj

# Publish
dotnet publish src/Namesearch.Api/Namesearch.Api.csproj -c Release

# Check solution structure
dotnet sln list
```

---

## 🔐 Security Checklist

- [ ] Replace API keys with Managed Identity in production
- [ ] Configure CORS properly for your domain
- [ ] Enable Application Insights for monitoring
- [ ] Set up Azure Key Vault for secrets
- [ ] Configure authentication if needed

---

## 📚 Documentation

- **README.md** - Full project documentation
- **DEPLOYMENT.md** - Detailed deployment guide
- **src/Namesearch.Web/README.md** - Component documentation
- **PROJECT_SUMMARY.md** - Implementation details
- **ARCHITECTURE.md** - Architecture overview

---

## 🐛 Troubleshooting

### UI doesn't load
- Check browser console (F12)
- Verify `ApiBaseUrl` in configuration
- Ensure both projects built successfully

### Search returns no results
- Verify Azure Search credentials
- Check if search index has data
- Review API logs

### Build fails
```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

---

## 💡 Tips

- Use **Ctrl+Shift+P** in VS Code for commands
- Press **F5** to debug
- Use **Developer Tools** (F12) to inspect network calls
- Check **Application Insights** in Azure Portal for production logs

---

## 🎯 Next Features to Add

- [ ] User authentication (Azure AD)
- [ ] Export search results to Excel
- [ ] Save search queries
- [ ] Advanced date filters
- [ ] Dark mode toggle
- [ ] Email notifications

---

## 📞 Getting Help

- Check the documentation files in the repo
- Review Azure Search documentation
- Check application logs
- Create an issue in the repository

---

**Made with ❤️ using Clean Architecture + Blazor Server + Azure**
