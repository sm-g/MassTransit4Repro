using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GreenPipes;
using MassTransit;
using Messages;
using Microsoft.Extensions.Logging;
using Topshelf;

namespace Consumer
{
    public class Service : ServiceControl
    {
        private IBusControl _busControl;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerFactory _loggerFactory;

        public Service(
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory
            )
        {
            _serviceProvider = serviceProvider;
            _loggerFactory = loggerFactory;
        }

        public bool Start(HostControl hostControl)
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(Configuration.RabbitHost), hst =>
                {
                    hst.Username("guest");
                    hst.Password("guest");
                });

                cfg.UseSerilog();

                cfg.ReceiveEndpoint(host, Configuration.QueueAName, e =>
                {
                    e.UseConcurrencyLimit(2);
                    e.PrefetchCount = 8;

                    e.Consumer<SimpleConsumer>();

                    e.BindMessageExchanges = false;
                    
                    e.Bind<IOperationARequested>(s =>
                    {
                        s.RoutingKey = Configuration.OperationARoutingKey;
                        s.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
                    });
                });

                cfg.ReceiveEndpoint(host, Configuration.QueueUser1Name, e =>
                {
                    e.UseConcurrencyLimit(2);
                    e.PrefetchCount = 8;

                    e.Consumer<SimpleConsumer>();

                    e.BindMessageExchanges = false;
                    
                    e.Bind<IOperationBRequested>(s =>
                    {
                        s.RoutingKey = "user1";
                        s.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
                    });
                });
            });

            _busControl.ConnectConsumeObserver(new ConsumeObserver(_loggerFactory.CreateLogger<ConsumeObserver>()));
            _busControl.ConnectPublishObserver(new PublishObserver(_loggerFactory.CreateLogger<PublishObserver>()));

            try
            {
                _busControl.Start();
            }
            catch
            {
                _busControl.Stop();

                throw;
            }

            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            _busControl?.Stop(TimeSpan.FromMinutes(3));

            return true;
        }
    }
}