using System.Reflection;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Pragmatic.Design.Core;

public static class EndpointRouteBuilderExtensions
{
    public static void MapQuery<TQuery>(this WebApplication app, Delegate queryHandler)
        where TQuery : IMessage
    {
        var queryType = typeof(TQuery);
        var routeAttribute = queryType.GetCustomAttribute<RouteAttribute>();
        var authorizeAttribute = queryType.GetCustomAttribute<AuthorizeAttribute>();

        var route = routeAttribute?.Template;
        var roles = authorizeAttribute?.Roles;

        if (route == null)
            return;

        var endpointBuilder = app.MapGet(route, queryHandler).WithOpenApi();

        var produces = queryType.GetCustomAttributes<ProducesAttribute>();

        foreach (var produce in produces)
            endpointBuilder.Produces(produce.StatusCode, produce.Type);

        if (!string.IsNullOrEmpty(roles))
            endpointBuilder.RequireAuthorization(new AuthorizeAttribute { Roles = roles });

        ApplyEndpointAttributes(endpointBuilder, queryHandler.Method);
    }

    public static void MapCommand<TCommand>(this WebApplication app, Delegate commandHandler, bool update)
        where TCommand : IMessage
    {
        var commandType = typeof(TCommand);
        var routeAttribute = commandType.GetCustomAttribute<RouteAttribute>();
        var authorizeAttribute = commandType.GetCustomAttribute<AuthorizeAttribute>();

        var route = routeAttribute?.Template;
        var roles = authorizeAttribute?.Roles;

        if (route != null)
        {
            var endpointBuilder = update ? app.MapPut(route, commandHandler) : app.MapPost(route, commandHandler);

            var produces = commandType.GetCustomAttributes<ProducesAttribute>();

            foreach (var produce in produces)
                endpointBuilder.Produces(produce.StatusCode, produce.Type);

            if (!string.IsNullOrEmpty(roles))
                endpointBuilder.RequireAuthorization(new AuthorizeAttribute { Roles = roles });

            ApplyEndpointAttributes(endpointBuilder, commandHandler.Method);
        }
    }

    private static void ApplyEndpointAttributes(RouteHandlerBuilder endpointBuilder, MethodInfo handlerMethod)
    {
        var attributes = handlerMethod.GetCustomAttributes();
        foreach (var attribute in attributes)
            switch (attribute)
            {
                case EndpointDescriptionAttribute descriptionAttribute:
                    endpointBuilder.WithDescription(descriptionAttribute.Description);
                    break;
                case EndpointSummaryAttribute summaryAttribute:
                    endpointBuilder.WithSummary(summaryAttribute.Summary);
                    break;
                case EndpointNameAttribute nameAttribute:
                    endpointBuilder.WithName(nameAttribute.EndpointName);
                    break;
                case EndpointGroupNameAttribute groupNameAttribute:
                    endpointBuilder.WithGroupName(groupNameAttribute.EndpointGroupName);
                    break;
            }
    }
}
