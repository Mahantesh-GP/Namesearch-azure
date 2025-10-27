using DocumentSummarizer.API.Configurations;
using DocumentSummarizer.API.Interfaces;
using DocumentSummarizer.API.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Register configuration options. These map to the "AzureOpenAI" section in appsettings.json
builder.Services.Configure<AzureOpenAIOptions>(builder.Configuration.GetSection("AzureOpenAI"));

// Register internal services for dependency injection. Implementations live in the Services folder.
builder.Services.AddTransient<IAzureSearchService, AzureSearchService>();
builder.Services.AddTransient<IQueryDocumentService, QueryDocumentService>();
builder.Services.AddTransient<ISearchService, SearchService>();

// Add controllers and API exploration support (Swagger/OpenAPI)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline. Swagger is enabled for development by default.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Map controller endpoints. See the Controllers folder for implementations.
app.MapControllers();

// Provide a simple root endpoint for sanity checks.
app.MapGet("/", () => "Document Summarizer API is running.");

app.Run();