# Namesearch API

[![.NET Build & Test](https://github.com/Mahantesh-GP/Namesearch-azure/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Mahantesh-GP/Namesearch-azure/actions/workflows/dotnet.yml)

Minimal ASP.NET Core API for name search backed by Azure Cognitive Search.

## Quick start

- Prerequisites: .NET 9 SDK
- Run locally (PowerShell):

```powershell
# from repo root
dotnet build .\Namesearch.sln -c Debug
dotnet run --project .\Namesearch\Namesearch.csproj
```

- Open:
  - Health: http://localhost:64347/healthz
  - Swagger: http://localhost:64347/swagger

## Configuration

The search service reads settings from the `AzureSearchServices` section.

```json
{
  "AzureSearchServices": {
    "Endpoint": "https://<your-service>.search.windows.net",
    "ApiKey": "<your-key>",
    "IndexName": "hybrid"
  }
}
```

Notes:
- In Development, options validation is enabled at startup. Provide these values in `appsettings.Development.json` or user secrets.
- In Production, startup succeeds without these values; the search endpoint will throw a clear error if configuration is missing.

## Testing

```powershell
# run all tests
dotnet test .\Namesearch.Tests\Namesearch.Tests.csproj --logger "trx;LogFileName=TestResults.trx"
```

Included tests:
- Health check: GET /healthz returns 200 OK
- Query endpoint (stubbed): POST /api/rag/query returns 200 and stub content

## Project layout

- `Namesearch/` – ASP.NET Core API
- `Namesearch.Tests/` – xUnit integration tests
- `.github/workflows/dotnet.yml` – CI build + test

## Endpoints

- `GET /healthz` – health probe
- `GET /swagger` – interactive API docs
- `POST /api/rag/query` – name search (requires Azure Search config)

## Notes

- Dependencies trimmed to essentials (`Azure.Search.Documents`, Swagger packages).
- Typed options (`AzureSearchOptions`) with dev-only validation.
- Safer DI and nullability defaults without breaking existing flow.
