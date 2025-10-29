using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Namesearch.Application;
using Namesearch.Application.Contracts;
using Namesearch.Application.Features.Search;
using Namesearch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks
builder.Services.AddHealthChecks();

// CORS
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin()
		      .AllowAnyMethod()
		      .AllowAnyHeader();
	});
});

// Problem details for standardized error responses
builder.Services.AddProblemDetails();

builder.Services.AddApplication();
// Register validators from Application assembly
builder.Services.AddValidatorsFromAssemblyContaining<Namesearch.Application.Validation.UserQueryRequestValidator>();
builder.Services.AddInfrastructure(builder.Configuration);

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

app.MapGet("/", () => Results.Redirect("/swagger"));

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
