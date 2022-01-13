using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MassTransitXunitPerf;

public class SimpleUnitTest
{
    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000001")]
    [InlineData("00000000-0000-0000-0000-000000000002")]
    [InlineData("00000000-0000-0000-0000-000000000003")]
    public async Task Test1Async(string eventId)
    {
        var services = new ServiceCollection()
            .AddMassTransitInMemoryTestHarness(cfg =>
            {
                cfg.AddConsumer<MyTestConsumer>();
                cfg.AddConsumerTestHarness<MyTestConsumer>();
            });

        var provider = services.BuildServiceProvider(true);

        var harness = provider.GetRequiredService<InMemoryTestHarness>();
        await harness.Start();
        try
        {
            var bus = provider.GetRequiredService<IBus>();

            await bus.Publish<MyTestEvent>(new { EventId = new Guid(eventId) });

            (await harness.Consumed.Any<MyTestEvent>(e => e.Context.Message.EventId == new Guid(eventId))).Should().BeTrue("The message consumed should be the message published");
            (await harness.Published.Any<Fault<MyTestEvent>>(e => e.Context.Message.Message.EventId == new Guid(eventId))).Should().BeFalse("We aren't expecting a fault");

            var consumerHarness = provider.GetRequiredService<IConsumerTestHarness<MyTestConsumer>>();
            (await consumerHarness.Consumed.Any<MyTestEvent>(e => e.Context.Message.EventId == new Guid(eventId))).Should().BeTrue("The consumer should have actually consumed this event");
        }
        finally
        {
            await harness.Stop();

            await provider.DisposeAsync();
        }
    }
}
