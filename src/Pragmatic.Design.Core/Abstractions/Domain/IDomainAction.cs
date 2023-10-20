using Mediator;

namespace Pragmatic.Design.Core.Abstractions.Domain;

public interface IDomainAction : IRequest
{
    public ValueTask<Unit> Handle(Root root, CancellationToken cancellationToken);
}

public interface IDomainAction<TResponse> : IRequest<TResponse>
{
    public ValueTask<TResponse> Handle(Root root, CancellationToken cancellationToken);
}
