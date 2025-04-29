using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MinimalApiResilience.Settings;
using Polly;

namespace MinimalApiResilience.ServiceCollectionExtensions
{    
    /// <summary>
    /// Extension methods for configuring HttpClient with resilience policies.
    /// </summary>
    public static class HttpClientExtensions
    {
        public static void AddExternalApiHttpClient(this IServiceCollection services)
        {
            services.AddOptions<ExternalApiSettings>()
                .BindConfiguration("ExternalApiSettings");

            services.AddHttpClient("ExternalApi", (serviceProvider, client) =>
            {
                var settings = GetSettings(serviceProvider);
                client.BaseAddress = new Uri(settings.BaseUrl);
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var settings = GetSettings(serviceProvider);

                var retryPolicy = ResiliencePoliciesExtensions.GetRetryPolicy(settings.RetryCount);
                var timeoutPolicy = ResiliencePoliciesExtensions.GetTimeoutPolicy(settings.TimeoutSeconds);
                var circuitBreakerPolicy = ResiliencePoliciesExtensions.GetCircuitBreakerPolicy(
                    settings.CircuitBreakerFailureThreshold,
                    settings.CircuitBreakerDurationSeconds
                );

                // Compose policies in a single execution chain
                return Policy.WrapAsync(retryPolicy, timeoutPolicy, circuitBreakerPolicy);
            });

            static ExternalApiSettings GetSettings(IServiceProvider serviceProvider)
            {
                return serviceProvider.GetRequiredService<IOptions<ExternalApiSettings>>().Value;
            }
        }
    }
}


