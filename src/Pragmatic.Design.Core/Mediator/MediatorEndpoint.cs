using FastEndpoints;
using Mediator;

namespace Pragmatic.Design.Core.Mediator;

public class MediatorEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : class
{
    private readonly IMediator _mediator;

    public MediatorEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(TRequest? req, CancellationToken ct)
    {
        if (req == null)
            throw new ArgumentNullException(nameof(req));
        await SendOkAsync((await _mediator.Send(req, ct) as TResponse)!, ct);
    }
}

public class MediatorEndpoint<TRequest> : Endpoint<TRequest>
    where TRequest : notnull
{
    private readonly IMediator _mediator;

    public MediatorEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(TRequest? req, CancellationToken ct)
    {
        if (req == null)
            throw new ArgumentNullException(nameof(req));
        await _mediator.Send(req, ct);
        await SendOkAsync(ct);
    }
}
