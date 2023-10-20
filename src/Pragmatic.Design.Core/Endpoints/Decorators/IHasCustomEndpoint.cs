namespace Pragmatic.Design.Core.Endpoints.Decorators;

public interface IHasCustomEndpoint : IHasEndpoint
{
    public Task Endpoint(EndpointArgs args, CancellationToken ct);
}
