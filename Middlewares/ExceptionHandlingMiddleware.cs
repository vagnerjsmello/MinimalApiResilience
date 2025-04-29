using Polly.CircuitBreaker;
using Polly.Timeout;

/// <summary>
/// Middleware to handle exceptions and provide custom error responses.
/// </summary>
namespace MinimalApiResilience.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning("BrokenCircuitException intercepted: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsJsonAsync(new { Error = "Service temporarily unavailable. Please try again later." });
            }
            catch (TimeoutRejectedException ex)
            {
                _logger.LogWarning("TimeoutRejectedException intercepted: {Message}", ex.Message);
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                await context.Response.WriteAsJsonAsync(new { Error = "The operation timed out. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { Error = $"An unexpected error occurred: {ex.Message}" });
            }
        }
    }
}
