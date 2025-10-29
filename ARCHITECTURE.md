# Architecture & Request Flow

This document explains the architecture and request flow for the Namesearch API, which follows Clean Architecture principles with CQRS and dependency inversion.

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Request Flow](#request-flow)
- [Layer Responsibilities](#layer-responsibilities)
- [Dependency Injection](#dependency-injection)
- [SOLID Principles](#solid-principles)
- [Testing Strategy](#testing-strategy)

---

## Architecture Overview

The solution is organized into four main layers:

```
┌─────────────────────────────────────────────────────────────┐
│  API Layer (Namesearch.Api)                                 │
│  - Minimal API endpoints                                     │
│  - Swagger/OpenAPI                                          │
│  - Middleware (CORS, health checks, error handling)         │
└────────────────┬────────────────────────────────────────────┘
                 │ depends on ↓
┌─────────────────────────────────────────────────────────────┐
│  Application Layer (Namesearch.Application)                │
│  - CQRS (MediatR queries/commands)                         │
│  - Use case handlers                                        │
│  - Validators (FluentValidation)                           │
│  - Contracts (DTOs)                                         │
│  - Abstractions (interfaces)                               │
└────────────────┬────────────────────────────────────────────┘
                 │ defines interfaces ↑
                 │ implemented by ↓
┌─────────────────────────────────────────────────────────────┐
│  Infrastructure Layer (Namesearch.Infrastructure)           │
│  - Azure Search implementation                              │
│  - External service adapters                                │
│  - Configuration (Options pattern)                          │
└────────────────┬────────────────────────────────────────────┘
                 │ depends on ↓
┌─────────────────────────────────────────────────────────────┐
│  Domain Layer (Namesearch.Domain)                          │
│  - Core business models (lightweight for now)               │
│  - Domain primitives                                        │
└─────────────────────────────────────────────────────────────┘
```

---

## Request Flow

### Complete Flow: POST /api/search

Here's how a search request flows through the system:

```
┌─────────────────────────────────────────────────────────────┐
│  1. HTTP Client                                             │
│     POST /api/search                                        │
│     Body: { "query": "John", "page": 1, "pageSize": 10 }   │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│  2. API Endpoint (Program.cs)                               │
│     - ASP.NET Core deserializes JSON → UserQueryRequest     │
│     - Injects IValidator<UserQueryRequest>                  │
│     - Injects ISender (MediatR)                             │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│  3. Validation (UserQueryRequestValidator)                  │
│     - Check query is not empty (min 2 chars)                │
│     - Check page >= 1                                       │
│     - Check pageSize between 1-100                          │
│                                                             │
│     ✗ Invalid → Return 400 Bad Request (ProblemDetails)    │
│     ✓ Valid → Continue                                      │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│  4. MediatR Dispatch                                        │
│     sender.Send(new QueryDocuments.Query(request))          │
│     → Routes to QueryDocuments.Handler                      │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│  5. Handler (QueryDocuments.Handler)                        │
│     - Receives Query                                        │
│     - Calls _search.SearchAsync(request)                    │
│     - Depends on IAzureSearchService interface              │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│  6. Infrastructure (AzureSearchService)                     │
│     - Build SearchOptions (pagination, filters, fields)     │
│     - Call Azure SDK: SearchClient.SearchAsync()            │
│     - Map SearchDocument → ResponseSummary DTOs             │
│     - Return IReadOnlyList<ResponseSummary>                 │
└────────────────────┬────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────┐
│  7. Azure Cognitive Search (External Service)               │
│     - Execute search query against index                    │
│     - Return search results with scores                     │
└────────────────────┬────────────────────────────────────────┘
                     │
                     │ Results bubble back up
                     ▼
┌─────────────────────────────────────────────────────────────┐
│  8. HTTP Response                                           │
│     200 OK                                                  │
│     [                                                       │
│       { "summary": "John Doe", "score": "0.85", ... },      │
│       { "summary": "John Smith", "score": "0.72", ... }     │
│     ]                                                       │
└─────────────────────────────────────────────────────────────┘
```

### Code Walkthrough

**1. API Endpoint (src/Namesearch.Api/Program.cs)**

```csharp
app.MapPost("/api/search", async (
    [FromBody] UserQueryRequest request, 
    ISender sender, 
    IValidator<UserQueryRequest> validator) =>
{
    // Step 1: Validate input
    ValidationResult result = await validator.ValidateAsync(request);
    if (!result.IsValid)
    {
        return Results.ValidationProblem(result.ToDictionary());
    }

    // Step 2: Dispatch via MediatR
    var response = await sender.Send(new QueryDocuments.Query(request));
    return Results.Ok(response);
})
.WithName("Search");
```

**2. Validator (src/Namesearch.Application/Validation/UserQueryRequestValidator.cs)**

```csharp
public sealed class UserQueryRequestValidator : AbstractValidator<UserQueryRequest>
{
    public UserQueryRequestValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("Query must be at least 2 characters.");

        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
```

**3. CQRS Handler (src/Namesearch.Application/Features/Search/QueryDocuments.cs)**

```csharp
public static class QueryDocuments
{
    // The Query (request contract)
    public sealed record Query(UserQueryRequest Request) 
        : IRequest<IReadOnlyList<ResponseSummary>>;

    // The Handler (use case orchestration)
    public sealed class Handler : IRequestHandler<Query, IReadOnlyList<ResponseSummary>>
    {
        private readonly IAzureSearchService _search;

        public Handler(IAzureSearchService search) => _search = search;

        public Task<IReadOnlyList<ResponseSummary>> Handle(
            Query request, 
            CancellationToken cancellationToken)
        {
            // Delegate to infrastructure service
            return _search.SearchAsync(request.Request, cancellationToken);
        }
    }
}
```

**4. Infrastructure Service (src/Namesearch.Infrastructure/Search/AzureSearchService.cs)**

```csharp
public sealed class AzureSearchService : IAzureSearchService
{
    private readonly SearchClient _client;
    private readonly ILogger<AzureSearchService> _logger;

    public AzureSearchService(
        IOptions<AzureSearchOptions> options, 
        ILogger<AzureSearchService> logger)
    {
        var opt = options.Value;
        _client = new SearchClient(
            new Uri(opt.Endpoint), 
            opt.IndexName, 
            new AzureKeyCredential(opt.ApiKey)
        );
        _logger = logger;
    }

    public async Task<IReadOnlyList<ResponseSummary>> SearchAsync(
        UserQueryRequest request, 
        CancellationToken ct = default)
    {
        var options = BuildSearchOptions(request);
        
        // Call Azure Cognitive Search SDK
        var results = await _client.SearchAsync<SearchDocument>(
            request.Query, 
            options, 
            ct
        );

        // Map to application DTOs
        var list = new List<ResponseSummary>();
        await foreach (var res in results.Value.GetResultsAsync().WithCancellation(ct))
        {
            list.Add(new ResponseSummary
            {
                Summary = GetString(res.Document, "fullName"),
                FileName = GetString(res.Document, "sourceId"),
                Score = res.Score?.ToString("0.00"),
                // ... map other fields
            });
        }

        _logger.LogInformation("Search returned {count} results", list.Count);
        return list;
    }

    private static SearchOptions BuildSearchOptions(UserQueryRequest request)
    {
        var page = Math.Max(1, request.Page);
        var pageSize = request.PageSize <= 0 ? 10 : Math.Min(100, request.PageSize);

        var options = new SearchOptions
        {
            Size = pageSize,
            Skip = (page - 1) * pageSize,
            IncludeTotalCount = true,
            SearchMode = SearchMode.Any,
            QueryType = SearchQueryType.Full
        };

        options.SearchFields.Add("fullName");
        options.OrderBy.Add("search.score() desc");
        return options;
    }
}
```

---

## Layer Responsibilities

### API Layer (Namesearch.Api)

**Purpose**: HTTP interface and application composition root

**Responsibilities**:
- Define HTTP endpoints (routes, methods)
- Request/response serialization (JSON)
- Middleware configuration (CORS, health checks, error handling)
- Dependency injection wiring
- Swagger/OpenAPI documentation

**Key Files**:
- `Program.cs` - Application startup and endpoint definitions
- `appsettings.json` - Configuration

**Dependencies**: Application, Infrastructure

---

### Application Layer (Namesearch.Application)

**Purpose**: Business logic and use cases

**Responsibilities**:
- Define use cases as CQRS queries/commands
- Orchestrate business workflows
- Define abstractions (interfaces) for external dependencies
- Define contracts (DTOs) for requests/responses
- Input validation rules

**Key Files**:
- `Features/Search/QueryDocuments.cs` - Search use case
- `Abstractions/IAzureSearchService.cs` - Search service interface
- `Contracts/UserQueryRequest.cs` - Request DTO
- `Contracts/ResponseSummary.cs` - Response DTO
- `Validation/UserQueryRequestValidator.cs` - Validation rules
- `DependencyInjection.cs` - Application services registration

**Dependencies**: Domain only (no infrastructure dependencies)

**Key Principle**: This layer defines interfaces but doesn't implement external dependencies—that's Infrastructure's job!

---

### Infrastructure Layer (Namesearch.Infrastructure)

**Purpose**: External integrations and technical implementations

**Responsibilities**:
- Implement application abstractions
- Integrate with external services (Azure Search, databases, APIs)
- Configure SDK clients
- Map between external models and application DTOs

**Key Files**:
- `Search/AzureSearchService.cs` - Implements IAzureSearchService
- `Configuration/AzureSearchOptions.cs` - Strongly-typed config
- `DependencyInjection.cs` - Infrastructure services registration

**Dependencies**: Application (implements interfaces), Domain, external SDKs

---

### Domain Layer (Namesearch.Domain)

**Purpose**: Core business models and rules

**Responsibilities**:
- Define domain entities
- Business invariants and rules
- Domain events (if needed)

**Current State**: Lightweight (just `GlobalUsings.cs` for now)

**Dependencies**: None (completely independent)

---

## Dependency Injection

### Startup Wiring (Program.cs)

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register services layer by layer
builder.Services.AddApplication();              // MediatR handlers
builder.Services.AddInfrastructure(configuration);  // Azure Search service
builder.Services.AddValidatorsFromAssemblyContaining<UserQueryRequestValidator>();

// Infrastructure services get registered as interfaces:
// - IAzureSearchService → AzureSearchService
// - IOptions<AzureSearchOptions> → bound from config
```

### Dependency Graph

```
API Endpoint
  ↓ depends on
ISender (MediatR) + IValidator<UserQueryRequest>
  ↓ MediatR resolves
QueryDocuments.Handler
  ↓ constructor injection
IAzureSearchService (interface in Application)
  ↓ DI container provides
AzureSearchService (concrete in Infrastructure)
  ↓ constructor injection
IOptions<AzureSearchOptions> + ILogger<AzureSearchService>
```

### Configuration Binding

```json
// appsettings.json
{
  "AzureSearchServices": {
    "Endpoint": "https://<your-service>.search.windows.net/",
    "ApiKey": "<your-key>",
    "IndexName": "hybrid"
  }
}
```

```csharp
// Infrastructure/DependencyInjection.cs
services.AddOptions<AzureSearchOptions>()
    .Bind(configuration.GetSection("AzureSearchServices"))
    .ValidateDataAnnotations();

services.AddSingleton<IAzureSearchService, AzureSearchService>();
```

---

## SOLID Principles

### Single Responsibility Principle

Each class has one reason to change:

- **Endpoint**: HTTP routing and serialization
- **Validator**: Input validation rules
- **Handler**: Use case orchestration
- **Service**: Azure Search integration

### Open/Closed Principle

- Add new features by adding new handlers (open for extension)
- Existing handlers don't need modification (closed for modification)

### Liskov Substitution Principle

- `IAzureSearchService` can be swapped with any implementation
- Tests use mocks, production uses Azure—both satisfy the contract

### Interface Segregation Principle

- Interfaces are focused: `IAzureSearchService` only defines search operations
- No fat interfaces with unused methods

### Dependency Inversion Principle

✅ **High-level modules (Application) don't depend on low-level modules (Infrastructure)**

```csharp
// Application defines the abstraction
public interface IAzureSearchService
{
    Task<IReadOnlyList<ResponseSummary>> SearchAsync(
        UserQueryRequest request, 
        CancellationToken ct = default
    );
}

// Infrastructure implements it
public sealed class AzureSearchService : IAzureSearchService
{
    // Implementation details using Azure SDK
}

// Handler depends on abstraction, not concretion
public sealed class Handler
{
    private readonly IAzureSearchService _search;  // Interface!
    
    public Handler(IAzureSearchService search) => _search = search;
}
```

**Benefits**:
- Easy to swap Azure Search for Elasticsearch or in-memory search
- Handler can be tested without Azure credentials
- Changes to Azure SDK don't affect Application layer

---

## Testing Strategy

### Unit Testing (Namesearch.UnitTests)

**What to Test**:
1. **Validators**: Ensure validation rules work correctly
2. **Handlers**: Test use case logic with mocked dependencies
3. **Mapping logic**: Verify DTO transformations

**Example: Testing Handler in Isolation**

```csharp
public class QueryDocumentsHandlerTests
{
    [Fact]
    public async Task Returns_results_from_search_service()
    {
        // Arrange: Mock the infrastructure
        var mockSearch = Substitute.For<IAzureSearchService>();
        var expected = new List<ResponseSummary> 
        { 
            new() { Summary = "John Doe" } 
        } as IReadOnlyList<ResponseSummary>;
        
        mockSearch.SearchAsync(Arg.Any<UserQueryRequest>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act: Test the handler with mock
        var handler = new QueryDocuments.Handler(mockSearch);
        var result = await handler.Handle(
            new QueryDocuments.Query(new UserQueryRequest { Query = "john" }), 
            CancellationToken.None
        );

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
```

**Example: Testing Validator**

```csharp
public class UserQueryRequestValidatorTests
{
    [Fact]
    public void Should_fail_when_query_is_empty()
    {
        var validator = new UserQueryRequestValidator();
        var result = validator.Validate(new UserQueryRequest { Query = "" });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_pass_for_valid_input()
    {
        var validator = new UserQueryRequestValidator();
        var result = validator.Validate(
            new UserQueryRequest { Query = "John", Page = 1, PageSize = 5 }
        );
        result.IsValid.Should().BeTrue();
    }
}
```

**Benefits**:
- No Azure credentials needed
- No network calls
- Fast execution
- Tests business logic in isolation

### Integration Testing (Future)

**Potential Additions**:
- Use `WebApplicationFactory` to test full HTTP pipeline
- Test endpoint → validator → handler → service flow
- Use in-memory or test Azure Search instance

---

## Key Architectural Benefits

### 1. Testability

- Mock interfaces for unit testing
- No need for real Azure Search in tests
- Fast test execution

### 2. Maintainability

- Clear separation of concerns
- Changes to infrastructure don't affect business logic
- Easy to locate code (feature slices)

### 3. Flexibility

- Swap Azure Search for another provider without touching Application
- Add new search features as new handlers
- Support multiple search providers simultaneously

### 4. Scalability

- Add features as vertical slices (new handlers)
- Each handler is independent
- Easy to parallelize development

### 5. Standards Compliance

- CQRS for clear command/query separation
- Clean Architecture for dependency flow
- SOLID principles throughout
- RFC 7807 ProblemDetails for errors

---

## Common Patterns Used

### CQRS (Command Query Responsibility Segregation)

- **Queries**: Read operations (QueryDocuments)
- **Commands**: Write operations (future: CreateIndex, UpdateDocument)
- Handled by MediatR

### Repository Pattern (Implicit)

- `IAzureSearchService` acts as a repository abstraction
- Hides data access details from application

### Options Pattern

- Strongly-typed configuration (`AzureSearchOptions`)
- Validated at startup with data annotations

### Adapter Pattern

- `AzureSearchService` adapts Azure SDK to application needs
- Maps between `SearchDocument` and `ResponseSummary`

---

## Future Enhancements

- **Structured Logging**: Add Serilog or OpenTelemetry
- **Caching**: Add response caching for frequently accessed searches
- **Resilience**: Add Polly for retry/circuit breaker policies
- **Pagination Metadata**: Return total count, page count in responses
- **Advanced Filters**: Support date ranges, facets, etc.
- **Integration Tests**: Add WebApplicationFactory tests
- **Performance Monitoring**: Add Application Insights telemetry

---

## Additional Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [Azure Cognitive Search .NET SDK](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/search.documents-readme)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

**Last Updated**: October 29, 2025
