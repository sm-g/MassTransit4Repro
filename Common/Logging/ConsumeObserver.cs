using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Common
{
    public class ConsumeObserver : IConsumeObserver
    {
        private readonly ILogger<ConsumeObserver> _logger;

        public ConsumeObserver(ILogger<ConsumeObserver> logger)
        {
            _logger = logger;
        }

        public Task PostConsume<T>(ConsumeContext<T> context) where T : class
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"PostConsume {context.MessageId} with dest address {context.DestinationAddress}");
            }

            return Task.CompletedTask;
        }

        public Task PreConsume<T>(ConsumeContext<T> context) where T : class
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"PreConsume {context.MessageId} with dest address {context.DestinationAddress}");
            }
            return Task.CompletedTask;
        }

        public Task ConsumeFault<T>(ConsumeContext<T> context, Exception exception) where T : class
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning(exception, $"ConsumeFault {context.MessageId} with dest address {context.DestinationAddress}");
            }

            return Task.CompletedTask;
        }
    }
}