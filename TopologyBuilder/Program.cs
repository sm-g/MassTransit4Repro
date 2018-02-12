using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using MassTransit;
using Messages;
using Microsoft.Extensions.DependencyInjection;

namespace TopologyBuilder
{
    internal class Program
    {
        private static void Main()
        {
            var contentRoot = Directory.GetCurrentDirectory();
            var cliArgs = new Dictionary<string, string>
            {
                [TopshelfCommon.EnvironmentArgName] = "Development"
            };
            var configuration = TopshelfCommon.BuildConfiguration(cliArgs, contentRoot);

            TopshelfCommon.SetupLogger(cliArgs, contentRoot, configuration, nameof(TopologyBuilder));

            var serviceProvider = configuration.BuildServiceProvider<Startup>();

            while (true)
            {
                Console.WriteLine("type routing key (username) and press Enter");
                var username = Console.ReadLine();

                var bus = CreateBus(username, serviceProvider);

                try
                {
                    bus.Start();
                }
                catch
                {
                    bus.Stop();

                    throw;
                }
                bus.Stop();
            }
        }

        private static IBusControl CreateBus(string username, IServiceProvider serviceProvider)
        {
            var result = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(Configuration.RabbitHost), hst =>
                {
                    hst.Username("guest");
                    hst.Password("guest");
                });

                cfg.UseSerilog();

                // will be created:
                // new queue, its exchange and binding from IOperationBRequested to new exchange
                cfg.ReceiveEndpoint(host, username + "_q", e =>
                {
                    e.BindMessageExchanges = false;

                    e.Bind<IOperationBRequested>( s =>
                    {
                        s.RoutingKey = username;
                        s.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
                    });
                });

                cfg.DeployTopologyOnly = true;
            });
            result.ConnectConsumeObserver(serviceProvider.GetRequiredService<ConsumeObserver>());
            result.ConnectPublishObserver(serviceProvider.GetRequiredService<PublishObserver>());
            return result;
        }
    }
}