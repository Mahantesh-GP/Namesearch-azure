using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namesearch.Application;
using Namesearch.Application.Contracts;
using Namesearch.Application.Features.Search;
using Namesearch.Infrastructure;
using Namesearch.Web.Components;
using Namesearch.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks
builder.Services.AddHealthChecks();

// CORS - updated for Blazor
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.WithOrigins("https://localhost:*", "http://localhost:*")
		      .AllowAnyMethod()
		      .AllowAnyHeader()
		      .AllowCredentials();
	});
});

// Problem details for standardized error responses
builder.Services.AddProblemDetails();

// Application and Infrastructure layers
builder.Services.AddApplication();
// Register validators from Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<Namesearch.Application.Validation.UserQueryRequestValidator>();
builder.Services.AddInfrastructure(builder.Configuration);

// Blazor Server components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HTTP Client for Blazor to call the API
builder.Services.AddHttpClient<ISearchApiClient, SearchApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7000");
});

var app = builder.Build();

// Middleware
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors();

// Health check endpoints
app.MapHealthChecks("/healthz");

// Serve Blazor UI
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Map Blazor components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// API endpoints
// Search endpoint (CQRS + Validation)
app.MapPost("/api/search", async ([FromBody] UserQueryRequest request, ISender sender, IValidator<UserQueryRequest> validator) =>
{
	ValidationResult result = await validator.ValidateAsync(request);
	if (!result.IsValid)
	{
		return Results.ValidationProblem(result.ToDictionary());
	}

	var response = await sender.Send(new QueryDocuments.Query(request));
	return Results.Ok(response);
})
.WithName("Search");

app.Run();
