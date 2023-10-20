namespace Pragmatic.Design.Core.Abstractions.Domain;

public class FakeDomainEventBus : IDomainEventBus
{
    public void Raise<TEvent>(TEvent ev)
        where TEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public void OnEvent<TEvent>(Action<TEvent> handler)
        where TEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public void Detach<TEvent>(Action<TEvent> handler)
        where TEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }

    public void DetachAll<TEvent>()
        where TEvent : IDomainEvent
    {
        throw new NotImplementedException();
    }
}
