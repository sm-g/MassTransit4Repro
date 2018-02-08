using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.RabbitMqTransport.Contexts;
using Microsoft.Extensions.Logging;

namespace Common
{
    public class PublishObserver : IPublishObserver
    {
        private readonly ILogger<PublishObserver> _logger;

        public PublishObserver(ILogger<PublishObserver> logger)
        {
            _logger = logger;
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"PostPublish {context.MessageId} with dest address {context.DestinationAddress}");
            }

            return Task.CompletedTask;
        }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            var rk = "";

            if (context.TryGetPayload<BasicPublishRabbitMqSendContext<T>>(out var basicPublishRabbitMqSendContext))
                rk = basicPublishRabbitMqSendContext.RoutingKey;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"PrePublish {context.MessageId}, RK='{rk}' with dest address {context.DestinationAddress}");
            }
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(exception, $"PublishFault {context.MessageId} with dest address {context.DestinationAddress}");
            }

            return Task.CompletedTask;
        }
    }
}