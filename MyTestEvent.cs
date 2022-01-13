using System;

namespace MassTransitXunitPerf;

public interface MyTestEvent
{
    public Guid EventId { get; }
}