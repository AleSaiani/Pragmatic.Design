using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pragmatic.Design.Core.Abstractions;
using Pragmatic.Design.Core.Abstractions.Domain;
using Pragmatic.Design.Core.Endpoints.Annotations;
using Pragmatic.Design.Core.Endpoints.Decorators;
using System.Reflection;

namespace Pragmatic.Design.Core.Endpoints;

/// <summary>
/// Base class for defining endpoints based on FastEndpoints and MediatR.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class MediatorEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : IDomainAction<TResponse>, IHasEndpoint, IRequest<TResponse>
{
    public override void Configure()
    {
        var tRequest = typeof(TRequest);
        string? httpMethod = null;
        string? template = null;

        // Detect the HTTP method attribute applied to the class
        // (HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch)
        // Set the HTTP method and routing template based on the detected attribute
        // (note: if both HttpGetAttribute and RouteAttribute are present, throw an exception)

        var getAttribute = tRequest.GetCustomAttribute<HttpGetAttribute>();
        if (getAttribute != null)
        {
            httpMethod = "GET";
            template = getAttribute.Template;
        }

        var postAttribute = tRequest.GetCustomAttribute<HttpPostAttribute>();
        if (postAttribute != null)
        {
            httpMethod = "POST";
            template = postAttribute.Template;
        }

        var putAttribute = tRequest.GetCustomAttribute<HttpPutAttribute>();
        if (putAttribute != null)
        {
            httpMethod = "PUT";
            template = putAttribute.Template;
        }

        var deleteAttribute = tRequest.GetCustomAttribute<HttpDeleteAttribute>();
        if (deleteAttribute != null)
        {
            httpMethod = "DELETE";
            template = deleteAttribute.Template;
        }

        var patchAttribute = tRequest.GetCustomAttribute<HttpPatchAttribute>();
        if (patchAttribute != null)
        {
            httpMethod = "PATCH";
            template = patchAttribute.Template;
        }

        if (httpMethod == null)
            throw new InvalidOperationException($"Endpoint {tRequest.Name} does not have any HTTP method attribute.");

        var routeAttribute = tRequest.GetCustomAttribute<RouteAttribute>();
        if (routeAttribute != null)
        {
            if (template != null)
                throw new InvalidOperationException($"Endpoint {tRequest.Name} cannot have both HTTP method attributes and a Route attribute with a template.");

            template = routeAttribute.Template;
        }

        if (template == null)
            throw new InvalidOperationException($"Endpoint {tRequest.Name} does not have a template. Use either Route or HTTP method attribute.");

        switch (httpMethod)
        {
            case "GET":
                Get(template);
                break;
            case "POST":
                Post(template);
                break;
            case "PUT":
                Put(template);
                break;
            case "DELETE":
                Delete(template);
                break;
            case "PATCH":
                Patch(template);
                break;
            default:
                throw new InvalidOperationException($"Unknown HTTP method: {httpMethod}");
        }

        // Handle authorization
        // If the endpoint has AllowAnonymousAttribute or does not have AuthorizeAttribute,
        // allow anonymous access
        // Otherwise, set the policies and authorized roles based on the AuthorizeAttribute
        var authorizeAttribute = tRequest.GetCustomAttribute<AuthorizeAttribute>();
        var allowAnonymous = tRequest.GetCustomAttribute<AllowAnonymousAttribute>();
        if (allowAnonymous != null || authorizeAttribute is null)
        {
            AllowAnonymous();
        }
        else
        {
            if (authorizeAttribute is { Policy: not null })
                Policies(authorizeAttribute.Policy);

            if (authorizeAttribute is { Roles: not null })
                Roles(authorizeAttribute.Roles);
        }

        var uploadAttribute = tRequest.GetCustomAttribute<EnableFileUploadAttribute>();
        if (uploadAttribute != null)
        {
            AllowFileUploads();
            RequestBinder(new MultipartFormRequestBinder<TRequest>());
        }

        // Handle Swagger information
        // Detect SwaggerResponseAttribute and SwaggerOperationAttribute attributes
        // Set description, summary, and Swagger options information based on detected attributes
        var swaggerResponseAttributes = tRequest.GetCustomAttributes<SwaggerResponseAttribute>().ToList();

        var swaggerOperationAttribute = tRequest.GetCustomAttribute<SwaggerOperationAttribute>();

        var props = tRequest.GetProperties();

        Description(d =>
        {
            d.WithName(swaggerOperationAttribute != null ? swaggerOperationAttribute.OperationId : tRequest.Name);

            if (props.Length == 0)
                d.ClearDefaultAccepts();

            foreach (var swaggerResponseAttribute in swaggerResponseAttributes)
                d.Produces(swaggerResponseAttribute.StatusCode, swaggerResponseAttribute.ResponseType, swaggerResponseAttribute.ContentType);

            if (!swaggerResponseAttributes.Any(a => a.StatusCode == 400))
                d.Produces(400, typeof(ProblemDetails));

            if (!swaggerResponseAttributes.Any(a => a.StatusCode == 500))
                d.Produces(500, typeof(ProblemDetails));
        });

        Summary(s =>
        {
            foreach (var swaggerResponseAttribute in swaggerResponseAttributes)
                if (!string.IsNullOrEmpty(swaggerResponseAttribute.Description))
                    s.Responses[swaggerResponseAttribute.StatusCode] = swaggerResponseAttribute.Description;

            if (swaggerOperationAttribute == null)
                return;
            if (swaggerOperationAttribute.Description != null)
                s.Description = swaggerOperationAttribute.Description;
            if (swaggerOperationAttribute.Summary != null)
                s.Summary = swaggerOperationAttribute.Summary;
        });
        Options(k =>
        {
            if (swaggerOperationAttribute == null)
                return;
            k.WithTags(swaggerOperationAttribute.Tags);
        });

        // Perform customizable additional configurations
        ExtraConfiguration();
    }

    /// <summary>
    /// Executes customizable additional configurations.
    /// </summary>
    protected virtual void ExtraConfiguration() { }

    /// <summary>
    /// Handles the request asynchronously.
    /// </summary>
    /// <param name="req">The request object.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(TRequest? req, CancellationToken ct)
    {
        if (req == null)
            throw new ArgumentNullException(nameof(req));

        if (req is not IHasEndpoint hasEndpoint)
            throw new InvalidOperationException($"Endpoint {req.GetType().Name} does not implement {nameof(IHasEndpoint)}.");

        // Verify if the request implements the IHasEndpoint interface
        // and if it implements the IHasCustomEndpoint interface
        // Execute custom endpoint logic if applicable
        // Otherwise, send the request to the mediator and send the response
        // as an HTTP OK response
        var preventDefault = false;
        var root = HttpContext.RequestServices.GetRequiredService<Root>();
        if (hasEndpoint is IHasCustomEndpoint endpoint)
            await endpoint.Endpoint(new EndpointArgs(root, () => preventDefault = true), ct);
        if (preventDefault)
            return;

        await SendOkAsync((await root.Mediator.Send(req, ct))!, ct);
    }
}

/// <summary>
/// Base class for defining endpoints based on FastEndpoints and MediatR.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
public class MediatorEndpoint<TRequest> : Endpoint<TRequest>
    where TRequest : IDomainAction, IHasEndpoint, IRequest
{
    public override void Configure()
    {
        var tRequest = typeof(TRequest);
        string? httpMethod = null;
        string? template = null;

        // Detect the HTTP method attribute applied to the class
        // (HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch)
        // Set the HTTP method and routing template based on the detected attribute
        // (note: if both HttpGetAttribute and RouteAttribute are present, throw an exception)

        var getAttribute = tRequest.GetCustomAttribute<HttpGetAttribute>();
        if (getAttribute != null)
        {
            httpMethod = "GET";
            template = getAttribute.Template;
        }

        var postAttribute = tRequest.GetCustomAttribute<HttpPostAttribute>();
        if (postAttribute != null)
        {
            httpMethod = "POST";
            template = postAttribute.Template;
        }

        var putAttribute = tRequest.GetCustomAttribute<HttpPutAttribute>();
        if (putAttribute != null)
        {
            httpMethod = "PUT";
            template = putAttribute.Template;
        }

        var deleteAttribute = tRequest.GetCustomAttribute<HttpDeleteAttribute>();
        if (deleteAttribute != null)
        {
            httpMethod = "DELETE";
            template = deleteAttribute.Template;
        }

        var patchAttribute = tRequest.GetCustomAttribute<HttpPatchAttribute>();
        if (patchAttribute != null)
        {
            httpMethod = "PATCH";
            template = patchAttribute.Template;
        }

        if (httpMethod == null)
            throw new InvalidOperationException($"Endpoint {tRequest.Name} does not have any HTTP method attribute.");

        var routeAttribute = tRequest.GetCustomAttribute<RouteAttribute>();
        if (routeAttribute != null)
        {
            if (template != null)
                throw new InvalidOperationException($"Endpoint {tRequest.Name} cannot have both HTTP method attributes and a Route attribute with a template.");

            template = routeAttribute.Template;
        }

        if (template == null)
            throw new InvalidOperationException($"Endpoint {tRequest.Name} does not have a template. Use either Route or HTTP method attribute.");

        switch (httpMethod)
        {
            case "GET":
                Get(template);
                break;
            case "POST":
                Post(template);
                break;
            case "PUT":
                Put(template);
                break;
            case "DELETE":
                Delete(template);
                break;
            case "PATCH":
                Patch(template);
                break;
            default:
                throw new InvalidOperationException($"Unknown HTTP method: {httpMethod}");
        }

        // Handle authorization
        // If the endpoint has AllowAnonymousAttribute or does not have AuthorizeAttribute,
        // allow anonymous access
        // Otherwise, set the policies and authorized roles based on the AuthorizeAttribute
        var authorizeAttribute = tRequest.GetCustomAttribute<AuthorizeAttribute>();
        var allowAnonymous = tRequest.GetCustomAttribute<AllowAnonymousAttribute>();
        if (allowAnonymous != null || authorizeAttribute is null)
        {
            AllowAnonymous();
        }
        else
        {
            if (authorizeAttribute is { Policy: not null })
                Policies(authorizeAttribute.Policy);

            if (authorizeAttribute is { Roles: not null })
                Roles(authorizeAttribute.Roles);
        }

        var uploadAttribute = tRequest.GetCustomAttribute<EnableFileUploadAttribute>();
        if (uploadAttribute != null)
        {
            AllowFileUploads();
            RequestBinder(new MultipartFormRequestBinder<TRequest>());
        }

        // Handle Swagger information
        // Detect SwaggerResponseAttribute and SwaggerOperationAttribute attributes
        // Set description, summary, and Swagger options information based on detected attributes
        var swaggerResponseAttributes = tRequest.GetCustomAttributes<SwaggerResponseAttribute>().ToList();

        var swaggerOperationAttribute = tRequest.GetCustomAttribute<SwaggerOperationAttribute>();

        var props = tRequest.GetProperties();

        Description(d =>
        {
            d.WithName(swaggerOperationAttribute != null ? swaggerOperationAttribute.OperationId : tRequest.Name);
            if (props.Length == 0)
                d.ClearDefaultAccepts();

            d.ClearDefaultProduces();
            foreach (var swaggerResponseAttribute in swaggerResponseAttributes)
                d.Produces(swaggerResponseAttribute.StatusCode, swaggerResponseAttribute.ResponseType, swaggerResponseAttribute.ContentType);

            bool has401Status = swaggerResponseAttributes.Any(a => a.StatusCode == 401);
            if (!has401Status && allowAnonymous == null && authorizeAttribute is not null)
                d.Produces(401);

            if (!swaggerResponseAttributes.Any(a => a.StatusCode == 200))
                d.Produces(200);

            if (!swaggerResponseAttributes.Any(a => a.StatusCode == 400))
                d.Produces(400, typeof(ProblemDetails));

            if (!swaggerResponseAttributes.Any(a => a.StatusCode == 500))
                d.Produces(500, typeof(ProblemDetails));
        });

        Summary(s =>
        {
            foreach (var swaggerResponseAttribute in swaggerResponseAttributes)
                if (!string.IsNullOrEmpty(swaggerResponseAttribute.Description))
                    s.Responses[swaggerResponseAttribute.StatusCode] = swaggerResponseAttribute.Description;

            if (swaggerOperationAttribute == null)
                return;
            if (swaggerOperationAttribute.Description != null)
                s.Description = swaggerOperationAttribute.Description;
            if (swaggerOperationAttribute.Summary != null)
                s.Summary = swaggerOperationAttribute.Summary;
        });
        Options(k =>
        {
            if (swaggerOperationAttribute == null)
                return;
            k.WithTags(swaggerOperationAttribute.Tags);
        });

        // Perform customizable additional configurations
        ExtraConfiguration();
    }

    /// <summary>
    /// Executes customizable additional configurations.
    /// </summary>
    protected virtual void ExtraConfiguration() { }

    /// <summary>
    /// Handles the request asynchronously.
    /// </summary>
    /// <param name="req">The request object.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(TRequest? req, CancellationToken ct)
    {
        if (req == null)
            throw new ArgumentNullException(nameof(req));

        if (req is not IHasEndpoint hasEndpoint)
            throw new InvalidOperationException($"Endpoint {req.GetType().Name} does not implement {nameof(IHasEndpoint)}.");

        // Verify if the request implements the IHasEndpoint interface
        // and if it implements the IHasCustomEndpoint interface
        // Execute custom endpoint logic if applicable
        // Otherwise, send the request to the mediator and send the response
        // as an HTTP OK response
        var preventDefault = false;
        var root = HttpContext.RequestServices.GetRequiredService<Root>();
        if (hasEndpoint is IHasCustomEndpoint endpoint)
            await endpoint.Endpoint(new EndpointArgs(root, () => preventDefault = true), ct);
        if (preventDefault)
            return;

        await root.Mediator.Send(req, ct);
        await SendOkAsync(ct);
    }
}
