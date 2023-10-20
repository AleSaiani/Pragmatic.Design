using FastEndpoints;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;

namespace Pragmatic.Design.Core.Mediator;

public class GlobalPostProcessor : IGlobalPostProcessor
{
    public Task PostProcessAsync(object req, object? res, HttpContext ctx, IReadOnlyCollection<ValidationFailure> failures, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
