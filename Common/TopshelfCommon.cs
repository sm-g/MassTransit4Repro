using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Topshelf.HostConfigurators;

namespace Common
{
    public interface IConsoleStartup
    {
        void ConfigureServices(IServiceCollection services);
    }

    public static class TopshelfCommon
    {
        // use values from Microsoft.AspNetCore.Hosting

        public const string EnvironmentArgName = "Environment";

        public static IReadOnlyDictionary<string, string> GetCliArgs(this HostConfigurator configurator)
        {
            var result = new Dictionary<string, string>
            {
                [EnvironmentArgName] = "Development"
            };
            // the only way to use command line args with topshelf:
            // -e:Production
            configurator.AddCommandLineDefinition("e", z => result[EnvironmentArgName] = z);
            configurator.ApplyCommandLine();

            return result;
        }

        public static IConfigurationRoot BuildConfiguration(IReadOnlyDictionary<string, string> cliArgs, string contentRoot)
        {
            return new ConfigurationBuilder()
                .SetBasePath(contentRoot)
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{cliArgs[EnvironmentArgName]}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddInMemoryCollection(cliArgs)
                .Build();
        }

        public static void SetupLogger(IReadOnlyDictionary<string, string> cliArgs, string contentRoot, IConfigurationRoot configuration, string serviceName)
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            logger.Information("Starting {ApplicationName} -e {EnvironmentName} from {ContentRootPath}",
                serviceName,
                cliArgs[EnvironmentArgName],
                contentRoot);

            Log.Logger = logger;
        }

        public static ServiceProvider BuildServiceProvider<TStartup>(this IConfigurationRoot configuration)
            where TStartup : class, IConsoleStartup
        {
            var serviceCollection = new ServiceCollection();

            var startup = Activator.CreateInstance(typeof(TStartup), configuration) as TStartup;
            serviceCollection.AddLogging(builder => builder.AddSerilog(dispose: true));
            startup.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        public static void SetServiceNames(this HostConfigurator configurator, string serviceName)
        {
            var name = $"{serviceName}";
            configurator.SetDescription(name);
            configurator.SetDisplayName(name);
            configurator.SetServiceName(name);
        }
    }
}