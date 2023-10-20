using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using Mediator;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Pragmatic.Design.Core.Abstractions.Domain;
using Pragmatic.Design.Core.Abstractions.Time;
using Pragmatic.Design.Core.Exceptions;

namespace Pragmatic.Design.Core.Abstractions;

public class Root
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public IMediator Mediator { get; }
    public DbContext DbContext { get; }
    public IDomainEventBus EventBus { get; }
    public IFeatureManager Features { get; }
    public IConfiguration Configuration { get; }
    public IDateTimeProvider DateTimeProvider { get; }
    public bool HasContext => _httpContextAccessor.HttpContext != null;

    public IServiceProvider ServiceProvider => _httpContextAccessor.HttpContext?.RequestServices ?? throw new InvalidOperationException("No HttpContext");

    public ClaimsPrincipal? CurrentUser => _httpContextAccessor.HttpContext?.User;
    public CultureInfo? CurrentCulture
    {
        get
        {
            var requestCulture = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
            return requestCulture?.RequestCulture.Culture;
        }
    }

    public CultureInfo? CurrentUICulture
    {
        get
        {
            var requestCulture = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
            return requestCulture?.RequestCulture.UICulture;
        }
    }

    public Root(
        IMediator mediator,
        IDomainEventBus eventBus,
        IFeatureManager features,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        IDateTimeProvider dateTimeProvider,
        DbContext dbContext
    )
    {
        _httpContextAccessor = httpContextAccessor;
        Mediator = mediator;
        EventBus = eventBus;
        Features = features;
        Configuration = configuration;
        DateTimeProvider = dateTimeProvider;
        DbContext = dbContext;
    }

    public void Log<T>(LogLevel logLevel, string message, [CallerMemberName] string caller = "", [CallerFilePath] string file = "")
    {
        var logger = ServiceProvider.GetService<ILogger<T>>();
        logger!.LogTrace($"Logger for {caller} in {file}");
        logger!.Log(logLevel, message);
    }

    public static async Task HandleException(HttpContext context)
    {
        var ex = context.Features.Get<IExceptionHandlerFeature>();
        if (ex != null)
        {
            Debug.WriteLine($"ERROR : {ex.Error.Message}");

            switch (ex.Error)
            {
                case UnauthorizedAccessException:
                    await Results.Unauthorized().ExecuteAsync(context);
                    break;
                case ForbiddenException:
                    await Results.Forbid().ExecuteAsync(context);
                    break;

                case NotFoundException:
                    await Results.NotFound().ExecuteAsync(context);
                    break;
                default:
                {
                    if (ex.Error is DomainException exception)
                    {
                        var details = exception.ToProblemDetails();
                        details.Status = (int?)HttpStatusCode.BadRequest;
                        await Results.Problem(details).ExecuteAsync(context);
                    }
                    else
                    {
                        var logger = context.RequestServices.GetService<ILogger<Root>>();
                        logger?.LogError(ex?.Error, "Unhandled exception");
                        await Results.StatusCode(500).ExecuteAsync(context);
                    }

                    break;
                }
            }
        }
    }
}
