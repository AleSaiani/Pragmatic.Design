using Mediator;
using Pragmatic.Design.Core.Abstractions;
using Pragmatic.Design.Core.Abstractions.Domain;

namespace Pragmatic.Design.Core.Endpoints;

/// <summary>
/// Base class for handling domain actions using MediatR.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class MediatorHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IDomainAction<TResponse>, IRequest<TResponse>
{
    private readonly Root _root;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatorHandler{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="root">The root instance providing access to application services.</param>
    protected MediatorHandler(Root root)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));
    }

    /// <summary>
    /// Handles the domain action asynchronously.
    /// </summary>
    /// <param name="domainAction">The domain action instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ValueTask{TResponse}"/> representing the asynchronous operation result.</returns>
    public ValueTask<TResponse> Handle(TRequest domainAction, CancellationToken cancellationToken)
    {
        if (domainAction == null)
            throw new ArgumentNullException(nameof(domainAction));

        // Call the Handle method on the domain action to perform the business logic.
        // Use the provided root instance and cancellation token.
        return domainAction.Handle(_root, cancellationToken);
    }
}

/// <summary>
/// Base class for handling domain actions using MediatR.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class MediatorHandler<TRequest> : IRequestHandler<TRequest>
    where TRequest : IDomainAction, IRequest
{
    private readonly Root _root;

    /// <summary>
    /// Initializes a new instance of the <see cref="MediatorHandler{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="root">The root instance providing access to application services.</param>
    protected MediatorHandler(Root root)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));
    }

    /// <summary>
    /// Handles the domain action asynchronously.
    /// </summary>
    /// <param name="domainAction">The domain action instance.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="ValueTask{TResponse}"/> representing the asynchronous operation result.</returns>
    public ValueTask<Unit> Handle(TRequest domainAction, CancellationToken cancellationToken)
    {
        if (domainAction == null)
            throw new ArgumentNullException(nameof(domainAction));

        // Call the Handle method on the domain action to perform the business logic.
        // Use the provided root instance and cancellation token.
        return domainAction.Handle(_root, cancellationToken);
    }
}
