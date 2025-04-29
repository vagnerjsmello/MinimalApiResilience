using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MinimalApiResilience.Settings;

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
                var settings = serviceProvider.GetRequiredService<IOptions<ExternalApiSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseUrl);
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<ExternalApiSettings>>().Value;
                return ResiliencePoliciesExtensions.GetRetryPolicy(settings.RetryCount);
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<ExternalApiSettings>>().Value;
                return ResiliencePoliciesExtensions.GetTimeoutPolicy(settings.TimeoutSeconds);
            })
            .AddPolicyHandler((serviceProvider, request) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<ExternalApiSettings>>().Value;
                return ResiliencePoliciesExtensions.GetCircuitBreakerPolicy(
                    settings.CircuitBreakerFailureThreshold,
                    settings.CircuitBreakerDurationSeconds
                );
            });
        }
    }
}
