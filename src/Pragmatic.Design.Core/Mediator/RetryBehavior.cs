using System.Reflection;
using Mediator;
using Polly;
using Pragmatic.Design.Core.Abstractions.Domain;

namespace Pragmatic.Design.Core.Mediator;

public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand
{
    public async ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        var retryAttribute = request.GetType().GetCustomAttribute<RetryPolicyAttribute>();

        if (retryAttribute == null)
            // Se la richiesta non ha l'attributo RetryPolicy, esegui normalmente
            return await next(request, cancellationToken);

        var policy = Policy
            .Handle<DomainException>()
            .WaitAndRetryAsync(retryAttribute.RetryCount, attempt => TimeSpan.FromSeconds(Math.Pow(retryAttribute.ExponentialBackoffFactor, attempt)));

        // Esegui la richiesta con la politica di retry
        return await policy.ExecuteAsync(async () => await next(request, cancellationToken));
    }
}
