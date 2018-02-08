using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Common
{
    public class BusObserver : IBusObserver
    {
        private readonly ILogger<BusObserver> _logger;

        public BusObserver(ILogger<BusObserver> logger)
        {
            _logger = logger;
        }

        public Task PostCreate(IBus bus)
        {
            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation($"Created bus {bus.Address}");
            return Task.CompletedTask;
        }

        public Task PreStart(IBus bus)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"Starting bus {bus.Address}");
            return Task.CompletedTask;
        }

        public Task PostStart(IBus bus, Task<BusReady> busReady)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"Started bus {bus.Address}");
            return Task.CompletedTask;
        }

        public Task PreStop(IBus bus)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"Stopping bus {bus.Address}");
            return Task.CompletedTask;
        }

        public Task PostStop(IBus bus)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"Stopped bus {bus.Address}");
            return Task.CompletedTask;
        }

        public Task CreateFaulted(Exception exception)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(exception, nameof(CreateFaulted));
            return Task.CompletedTask;
        }

        public Task StartFaulted(IBus bus, Exception exception)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(exception, $"Starting bus {bus.Address} failed");
            return Task.CompletedTask;
        }

        public virtual Task StopFaulted(IBus bus, Exception exception)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(exception, $"Stopping bus {bus.Address} failed");
            return Task.CompletedTask;
        }
    }
}