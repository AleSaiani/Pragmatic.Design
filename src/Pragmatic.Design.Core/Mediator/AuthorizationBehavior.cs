using System.Reflection;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Pragmatic.Design.Core.Mediator;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IMessage
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehavior(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
    {
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken, MessageHandlerDelegate<TRequest, TResponse> next)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>().ToList();

        if (!authorizeAttributes.Any())
            return await next(request, cancellationToken);

        foreach (var attribute in authorizeAttributes)
            if (_httpContextAccessor.HttpContext != null)
            {
                var user = _httpContextAccessor.HttpContext.User;

                if (!string.IsNullOrEmpty(attribute.Roles))
                {
                    var roles = attribute.Roles.Split(',');

                    if (!roles.Any(role => user.IsInRole(role.Trim())))
                        throw new UnauthorizedAccessException("You don't have permission to perform this action");
                }

                if (string.IsNullOrEmpty(attribute.Policy))
                    continue;
                var authorizationResult = await _authorizationService.AuthorizeAsync(user, attribute.Policy);

                if (!authorizationResult.Succeeded)
                    throw new UnauthorizedAccessException("You don't have permission to perform this action");
            }
            else
            {
                throw new UnauthorizedAccessException("You don't have permission to perform this action");
            }

        return await next(request, cancellationToken);
    }
}
