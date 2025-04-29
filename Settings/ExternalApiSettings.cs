namespace MinimalApiResilience.Settings
{
    /// <summary>
    /// Settings for the external API client.
    /// This class is used to bind configuration settings from appsettings.json or other configuration sources.
    /// </summary>
    public class ExternalApiSettings
    {
        public required string BaseUrl { get; set; }
        public int RetryCount { get; set; }
        public int TimeoutSeconds { get; set; }
        public int CircuitBreakerFailureThreshold { get; set; }
        public int CircuitBreakerDurationSeconds { get; set; }
    }
}
