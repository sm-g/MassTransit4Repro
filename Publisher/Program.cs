using System;
using System.Collections.Generic;
using System.IO;
using Common;
using MassTransit;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Topshelf;

namespace Publisher
{
    internal static class Program
    {
        private static void Main()
        {
            var contentRoot = Directory.GetCurrentDirectory();
            var cliArgs = new Dictionary<string, string>
            {
                [TopshelfCommon.EnvironmentArgName] = "Development"
            };
            var configuration = TopshelfCommon.BuildConfiguration(cliArgs, contentRoot);

            TopshelfCommon.SetupLogger(cliArgs, contentRoot, configuration, nameof(Publisher));

            var serviceProvider = configuration.BuildServiceProvider<Startup>();

            var _busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri(Configuration.RabbitHost), hst =>
                {
                    hst.Username("guest");
                    hst.Password("guest");
                });

                cfg.UseSerilog();

                // this does not change anything
                cfg.PublishTopology.BrokerTopologyOptions = MassTransit.RabbitMqTransport.Topology.PublishBrokerTopologyOptions.MaintainHierarchy;

                cfg.Publish<IOperationARequested>(x =>
                {
                    x.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
                });
                cfg.Publish<IOperationBRequested>(x =>
                {
                    x.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
                });
                                
                cfg.Send<IOperationARequested>(x =>
                {
                    x.UseRoutingKeyFormatter(context => Configuration.OperationARoutingKey);
                });                
                cfg.Send<IOperationBRequested>(x =>
                {
                    x.UseRoutingKeyFormatter(context => context.Message.Username);
                });
            });

            _busControl.ConnectConsumeObserver(serviceProvider.GetRequiredService<ConsumeObserver>());
            _busControl.ConnectPublishObserver(serviceProvider.GetRequiredService<PublishObserver>());

            try
            {
                _busControl.Start();
            }
            catch
            {
                _busControl.Stop();

                throw;
            }

            Console.WriteLine("press to publish");
            Console.ReadKey();

            _busControl.Publish(new OperationARequestedEvent());
            _busControl.Publish(new OperationBRequestedEvent { Username = "user1" });

            Console.WriteLine("published, press to stop");
            Console.ReadKey();

            _busControl.Stop();
        }
    }
}