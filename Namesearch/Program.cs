// Program.cs � .NET 8 minimal hosting

using Azure;
// using Azure.AI.OpenAI; // unused currently
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using DocumentSummarizer.API.Configurations;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------------------
// Logging
// ----------------------------------------------------------------------------
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ----------------------------------------------------------------------------
// Configuration binding
// ----------------------------------------------------------------------------
builder.Services.Configure<AzureOpenAIOptions>(
    builder.Configuration.GetSection("AzureOpenAI"));

// Bind AzureSearchOptions (no ValidateOnStart to avoid breaking startup without config)
builder.Services.Configure<AzureSearchOptions>(
    builder.Configuration.GetSection("AzureSearchServices"));

// In development, validate options on start to catch misconfiguration early without impacting prod
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOptions<AzureSearchOptions>()
        .Bind(builder.Configuration.GetSection("AzureSearchServices"))
        .ValidateDataAnnotations()
        .ValidateOnStart();
}

// Optional: read Azure Search settings from appsettings.json
// "AzureSearchServices": { "Endpoint": "...", "ApiKey": "...", "IndexName": "hybrid" }
var cfg = builder.Configuration;
var searchEndpoint = cfg["AzureSearchServices:Endpoint"] ?? "";
var searchKey = cfg["AzureSearchServices:ApiKey"] ?? "";
var indexName = cfg["AzureSearchServices:IndexName"] ?? "hybrid";

// ----------------------------------------------------------------------------
// SDK clients (singletons)
// ----------------------------------------------------------------------------
if (!string.IsNullOrWhiteSpace(searchEndpoint) && !string.IsNullOrWhiteSpace(searchKey))
{
    builder.Services.AddSingleton(new SearchClient(new Uri(searchEndpoint), indexName, new AzureKeyCredential(searchKey)));
    builder.Services.AddSingleton(new SearchIndexClient(new Uri(searchEndpoint), new AzureKeyCredential(searchKey)));
}

// Azure OpenAI client registration removed (unused). Re-add when needed with proper Options validation.

// Also a named HttpClient pointing to your Search service (handy for custom REST calls)
builder.Services.AddHttpClient("AzureSearch",
    http => http.BaseAddress = string.IsNullOrWhiteSpace(searchEndpoint) ? null : new Uri(searchEndpoint));

// ----------------------------------------------------------------------------
// App services + MVC bits
// ----------------------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// DI for your app services
builder.Services.AddTransient<IAzureSearchService, AzureSearchService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IQueryDocumentService, QueryDocumentService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", b =>
    {
        b.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader();
    });
});

// Swagger/OpenAPI (enabled in all environments)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Name Search API",
        Version = "v1",
        Description = "API for searching names via Azure Cognitive Search"
    });
});

var app = builder.Build();

// ----------------------------------------------------------------------------
// Pipeline
// ----------------------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseCors("CorsPolicy");

// Swagger UI visible everywhere (Azure included)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Name Search API v1");
    c.RoutePrefix = "swagger"; // UI at /swagger
});

// Minimal health + root redirect
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/healthz", () => Results.Ok("OK"));

app.MapRazorPages();
app.MapControllers();

app.Run();

// Expose Program for WebApplicationFactory in tests
public partial class Program { }
