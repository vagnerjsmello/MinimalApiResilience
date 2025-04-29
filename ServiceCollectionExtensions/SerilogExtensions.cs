using Serilog;
using Serilog.Events;

/// <summary>
/// Extensions for configuring Serilog in the application host builder.
/// </summary>
namespace MinimalApiResilience.ServiceCollectionExtensions
{
    public static class SerilogExtensions
    {
        public static void AddCustomSerilog(this IHostBuilder hostBuilder)
        {
            hostBuilder.UseSerilog((context, services, configuration) =>
            {
                configuration  // Important: Read from appsettings.json
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();
            });
        }
    }
}
