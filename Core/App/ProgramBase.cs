using Lens.Core.Lib.Services;
using Lamar.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Lens.Core.App
{
    public class ProgramBase
    {
        /// <summary>
        /// Allows the overriding class to add additional configuration sources while settting up the Logging Configuration
        /// </summary>
        protected static Action<IConfigurationBuilder> LoggingConfigurationSetup { get; set; }

        /// <summary>
        /// Allows to add configuration to serilogger, while it's being built. 
        /// The bool is if the logger configuration is about the bootstrapped logger, or the real application logger.
        /// </summary>
        protected static Action<LoggerConfiguration, bool> SeriloggerConfigurationSetup { get; set; }

        /// <summary>
        /// Allows the overriding class to add additional configuration sources while settting up the Host Configuration
        /// </summary>
        protected static Action<IConfigurationBuilder> HostConfigurationSetup { get; set; }

        /// <summary>
        /// Allows the overriding class to add additional configuration sources while settting up the App Configuration
        /// </summary>
        protected static Action<IConfigurationBuilder> AppConfigurationSetup { get; set; }

        public static async Task<int> Start(string[] args, Func<string[], IHostBuilder> appHostBuilder)
        {
            SetupStaticLogging();

            try
            {
                Log.Information("Starting host builder");

                IHostBuilder hostBuilder = appHostBuilder(args);

                var host = hostBuilder.Build();

                await Initialize(host);

                await host.RunAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        protected static IHostBuilder CreateBaseHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Use Lamar as the depencency injection framework
                .UseLamar()
                // Allow the deriving class to add extra configuration sources as early as possible while the program is starting up.
                .ConfigureHostConfiguration(config =>
                {
                    HostConfigurationSetup?.Invoke(config);
                })
                // Add Initialize properties from special json config file
                // Allow for the deriving class to add extra configuration sources for use while the program is running.
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.ProgramInitialize.json", optional: true);

                    AppConfigurationSetup?.Invoke(config);
                })

                // Use Serilog as the logging framework
                .UseSerilog((context, services, configuration) =>
                {
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services);


                    SeriloggerConfigurationSetup?.Invoke(configuration, false);

                });

        /// <summary>
        /// Setup static Serilog logging from configuration, on startup.
        /// </summary>
        private static void SetupStaticLogging()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configurationBuilder.AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                optional: true);

            LoggingConfigurationSetup?.Invoke(configurationBuilder);

            var configuration = configurationBuilder.Build();

            //See: https://nblumhardt.com/2020/10/bootstrap-logger/
            var logConfig = new LoggerConfiguration()
                .WriteTo.Console() // for the bootstrapped logger
                .ReadFrom.Configuration(configuration);


            SeriloggerConfigurationSetup?.Invoke(logConfig, true);

            Log.Logger = logConfig.CreateBootstrapLogger();
        }

        /// <summary>
        /// This method will fetch all IProgramStartupInitializer services and run Initialize on them. 
        /// This way libraries can hook into the application startup process and do some initialization 
        /// before the actual application loop runs.
        /// </summary>
        private static async Task Initialize(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var initializers = scope.ServiceProvider.GetServices<IProgramInitializer>();
            var logger = scope.ServiceProvider.GetService<ILogger<ProgramBase>>();
            foreach (var initializer in initializers)
            {
                try
                {
                    await initializer.Initialize();
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error running initializer {initializer}", initializer.GetType().Name);
                }
            }
        }
    }
}
