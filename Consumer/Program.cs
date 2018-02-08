using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Extensions.DependencyInjection;
using Topshelf;

namespace Consumer
{
    internal static class Program
    {
        private static void Main()
        {
            HostFactory.Run(x =>
            {
                var cliArgs = x.GetCliArgs();

                var contentRoot = Directory.GetCurrentDirectory();

                var configuration = TopshelfCommon.BuildConfiguration(cliArgs, contentRoot);

                TopshelfCommon.SetupLogger(cliArgs, contentRoot, configuration, nameof(SimpleConsumer));

                var serviceProvider = configuration.BuildServiceProvider<Startup>();

                x.Service(serviceProvider.GetRequiredService<Service>);

                x.UseSerilog();
                x.DependsOn("RabbitMQ");

                x.RunAsLocalSystem();
                x.StartAutomatically();

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(1);

                    // number of days until the error count resets
                    r.SetResetPeriod(1);
                });
            });
        }
    }
}