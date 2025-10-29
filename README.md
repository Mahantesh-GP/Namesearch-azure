# Namesearch (modernized)

[![.NET Build & Test](https://github.com/Mahantesh-GP/Namesearch-azure/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/Mahantesh-GP/Namesearch-azure/actions/workflows/dotnet.yml)

This repository has been refactored to a Clean Architecture layout with SOLID-friendly separation of concerns, CQRS with MediatR, FluentValidation, and an Infrastructure adapter for Azure Cognitive Search.

## New project structure

- `src/Namesearch.Domain` – core domain primitives (lightweight for now)
- `src/Namesearch.Application` – use cases (CQRS, validators, abstractions)
- `src/Namesearch.Infrastructure` – Azure Search implementation, options, DI
- `src/Namesearch.Api` – minimal API, Swagger, composition root
- `tests/Namesearch.UnitTests` – unit tests for validators and handlers

**Legacy `Namesearch/` and `Namesearch.Tests/` projects have been removed.**

## Quick start

Prerequisites: .NET 8 SDK

```powershell
# from repo root
dotnet build .\Namesearch.sln -c Debug
dotnet run --project .\src\Namesearch.Api\Namesearch.Api.csproj
```

Open:

- Swagger: http://localhost:5000/swagger (or the port shown in console)
- Health: http://localhost:5000/healthz

## Configuration

Set your Azure Cognitive Search settings under the `AzureSearchServices` section (see `src/Namesearch.Api/appsettings.json`):

```json
{
  "AzureSearchServices": {
    "Endpoint": "https://<your-search-service>.search.windows.net/",
    "ApiKey": "<your-api-key>",
    "IndexName": "hybrid"
  }
}
```

You can place secrets in `appsettings.Development.json` or user-secrets.

## API

- `GET /` – Redirects to Swagger UI
- `GET /healthz` – Health check (returns 200 Healthy)
- `POST /api/search` – Search endpoint
  - Body: `{ "query": "John", "page": 1, "pageSize": 10 }`
  - Returns a list of `ResponseSummary` objects

Validation is enforced via FluentValidation.

## Middleware features

- **Health checks**: `/healthz` endpoint for readiness/liveness probes
- **CORS**: Default policy allows any origin/method/header (configurable)
- **ProblemDetails**: Standardized error responses for exceptions and validation failures

## Testing

```powershell
dotnet test .\tests\Namesearch.UnitTests\Namesearch.UnitTests.csproj
```

All tests run in-memory with mocked dependencies.

## Design notes

- CQRS via MediatR: features are defined as vertically-sliced request/handler pairs.
- Clean DI boundaries: Application contains abstractions; Infrastructure provides implementations and is wired in the API.
- Typed options with data annotations for Azure Search configuration.
- Global properties (nullable, implicit usings) set in `Directory.Build.props`.

## Next steps

- Add integration tests using WebApplicationFactory for the new API if desired.
- Introduce structured logging (Serilog) or OpenTelemetry for tracing.
- Add pagination metadata to responses (total count, page count).
