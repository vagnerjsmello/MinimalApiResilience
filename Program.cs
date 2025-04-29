using MinimalApiResilience.ServiceCollectionExtensions;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog logging
builder.Host.AddCustomSerilog();

// Add HttpClient with resilience policies
builder.Services.AddExternalApiHttpClient();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware for exception handling
app.UseMiddleware<MinimalApiResilience.Middlewares.ExceptionHandlingMiddleware>();

// Enable Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Endpoint
app.MapGet("/data", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("ExternalApi");
    var response = await client.GetAsync("endpoint");
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    return Results.Ok(content);
})
.WithName("GetExternalData") // Optional: give a nice name for Swagger UI
.WithOpenApi();              // Makes it show in Swagger

              
app.Run();
