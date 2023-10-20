using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pragmatic.Design.Core.Abstractions.Domain;
using Pragmatic.Design.Core.Abstractions.Time;
using Pragmatic.Design.Core.Mediator;
using Pragmatic.Design.Core.Persistence;

namespace Pragmatic.Design.Core;

public static class CoreModule
{
    public static void RegisterCore<TDbContext>(this WebApplicationBuilder builder)
        where TDbContext : DbContext
    {
        builder.Services.AddScoped<IDomainEventBus, FakeDomainEventBus>();
        builder.Services.AddScoped<DbContext, TDbContext>();
        builder.Services.AddScoped<IDateTimeProvider, FakeDateTimeProvider>();
        // Register the ValidationBehavior pipeline behavior, which will validate commands and queries before they're handled.
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }

    public static void RegisterCore(this WebApplicationBuilder builder)
    {
        builder.RegisterCore<ApplicationDbContext>();
    }
}
