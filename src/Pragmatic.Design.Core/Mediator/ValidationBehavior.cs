using FluentValidation;
using FluentValidation.Results;
using Mediator;
using ValidationException = Pragmatic.Design.Core.Exceptions.ValidationException;

namespace Pragmatic.Design.Core.Mediator;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(TRequest message, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        if (_validators.Any())
        {
            ValidationContext<TRequest> validationContext = new(message);

            List<ValidationFailure> validationErrors = new();

            foreach (var validator in _validators)
            {
                var validationResult = await validator.ValidateAsync(validationContext, cancellationToken);

                if (validationResult.Errors != null)
                    validationErrors.AddRange(validationResult.Errors);
            }

            if (validationErrors.Any())
                throw new ValidationException(validationErrors.Select(e => e.ErrorMessage).ToList());
        }

        return await next(message, cancellationToken);
    }
}
