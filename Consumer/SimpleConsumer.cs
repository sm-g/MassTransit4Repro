using System.Threading.Tasks;
using MassTransit;
using Messages;

namespace Consumer
{
    public class SimpleConsumer : IConsumer<IOperationARequested>, IConsumer<IOperationBRequested>
    {
        public Task Consume(ConsumeContext<IOperationARequested> context)
        {
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<IOperationBRequested> context)
        {
            return Task.CompletedTask;
        }
    }
}