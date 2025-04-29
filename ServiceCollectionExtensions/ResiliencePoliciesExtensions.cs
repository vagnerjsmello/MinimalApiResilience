using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Serilog;

namespace MinimalApiResilience.ServiceCollectionExtensions
{
    /// <summary>
    /// Extension methods for configuring resilience policies for HTTP clients.    
    /// </summary>
    public static class ResiliencePoliciesExtensions
    {
        public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (outcome, timespan, retryAttempt, context) =>
                    {
                        Log.Warning($"Retry attempt {retryAttempt} failed. Waiting {timespan} before next attempt.");
                    });
        }

        public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(int timeoutSeconds)
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(timeoutSeconds);
        }

        public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy(int failuresAllowedBeforeBreaking, int durationOfBreakSeconds)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: failuresAllowedBeforeBreaking,
                    durationOfBreak: TimeSpan.FromSeconds(durationOfBreakSeconds),
                    onBreak: (outcome, timespan) =>
                    {
                        Log.Warning($"Circuit opened for {timespan.TotalSeconds} seconds due to: {outcome.Exception?.Message ?? outcome.Result.ReasonPhrase}");
                    },
                    onReset: () => Log.Information("Circuit closed. Operations resumed."),
                    onHalfOpen: () => Log.Information("Circuit half-open. Testing the system.")
                );
        }
    }
}
