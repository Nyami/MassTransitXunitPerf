using MassTransit;
using System.Threading.Tasks;

namespace MassTransitXunitPerf;

public class MyTestConsumer : IConsumer<MyTestEvent>
{
    public Task Consume(ConsumeContext<MyTestEvent> context)
    {
        var message = context.Message;

        return Task.CompletedTask;
    }
}
