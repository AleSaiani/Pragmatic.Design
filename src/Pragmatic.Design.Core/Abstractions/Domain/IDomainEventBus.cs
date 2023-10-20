namespace Pragmatic.Design.Core.Abstractions.Domain;

public interface IDomainEventBus
{
    void Raise<TEvent>(TEvent ev)
        where TEvent : IDomainEvent;
    void OnEvent<TEvent>(Action<TEvent> handler)
        where TEvent : IDomainEvent;
    void Detach<TEvent>(Action<TEvent> handler)
        where TEvent : IDomainEvent;
    void DetachAll<TEvent>()
        where TEvent : IDomainEvent;
}
