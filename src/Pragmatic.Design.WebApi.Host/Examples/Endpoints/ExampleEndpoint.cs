using FastEndpoints;

namespace Pragmatic.Design.WebApi.Host.Examples.Endpoints;

public class ExampleEndpoint : Endpoint<string, string>
{
    public override void Configure()
    {
        Post("/example");
        AllowAnonymous();
    }

    public override async Task HandleAsync(string req, CancellationToken ct)
    {
        await SendAsync("");
    }
}
