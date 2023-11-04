using System.Reflection;
using FastEndpoints;
using FastEndpoints.ClientGen;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Pragmatic.Design.Core;
using Pragmatic.Design.Core.Abstractions;
using Pragmatic.Design.Core.Bootstrap;
using Pragmatic.Design.Core.Infrastructure;
using Pragmatic.Design.Core.Persistence;
using Pragmatic.Design.DataProcessor;
using Pragmatic.Design.DataProcessor.Fixture;
using Pragmatic.Design.DataProcessor.Seeds;
using Pragmatic.Design.WebApi.Host.Examples.DataProcessor;
using Serilog;
using WatchDog;

try
{
    // Create the application builder
    var builder = WebApplication.CreateBuilder(args);
    var env = builder.Environment;

    // Check if running in Azure Web App
    var isRunningInAzureWebApp =
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HOME")) && !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));

    // Load necessary assemblies
    Assembly.Load("Pragmatic.Design.WebApi.Host");
    var allAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

    allAssemblies.RemoveAll(
        assembly =>
            assembly.FullName != null
            && (
                assembly.FullName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase)
                || assembly.FullName.StartsWith("FastEndpoints.", StringComparison.OrdinalIgnoreCase)
                || assembly.FullName.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
            )
    );

    // Configure logging, CORS, Swagger, and localization
    Startup.ConfigureLogging(builder, env);
    Startup.ConfigureCors(builder, env, isRunningInAzureWebApp);
    Startup.ConfigureSwagger(builder);
    Startup.ConfigureLocalization(builder);

    // Configure feature management and JSON serialization options
    builder.Services.AddFeatureManagement(builder.Configuration);
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.WriteIndented = true;
        options.SerializerOptions.IncludeFields = true;
    });

    // Add API endpoints and HTTP context accessor
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddFastEndpoints();

    // Add Mediator and authorization
    builder.Services.AddMediator(k => k.ServiceLifetime = ServiceLifetime.Scoped);
    builder.Services.AddAuthorization();

    // Add WatchDog services if not in integration testing mode
    if (!env.IsIntegrationTesting())
        builder.Services.AddWatchDogServices(opt =>
        {
            opt.IsAutoClear = true;
        });

    // Add Entity Framework DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(
        options => options.UseNpgsql(builder.Configuration.GetConnectionString("Database"), opt => opt.MigrationsAssembly(typeof(Program).Assembly.FullName))
    );

    // Register core services and configure DI discovery
    builder.RegisterCore();
    Startup.ConfigureDIDiscovery(builder, allAssemblies);

    builder.Services.AddScoped<ISeed, ExampleSeed>();
    builder.Services.AddScoped<IFixture, ExampleSqlFixture>();
    builder.Services.AddDataProcessor(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Examples", "DataProcessor"));

    // Build the application
    var app = builder.Build();

    // Configure exception handling and logging middleware
    app.UseExceptionHandler(new ExceptionHandlerOptions { ExceptionHandler = async context => await Root.HandleException(context), AllowStatusCode404Response = true });

    if (!env.IsIntegrationTesting())
    {
        app.UseMiddleware<LogEnrichmentMiddleware>();
        app.UseSerilogRequestLogging();
    }

    // Redirect to HTTPS and configure routing
    app.UseHttpsRedirection();
    app.UseRouting();

    // Configure CORS, request localization, and HSTS
    if (!isRunningInAzureWebApp && !env.IsIntegrationTesting())
        app.UseCors("Default");
    app.UseRequestLocalization();

    if (app.Environment.IsProduction())
        app.UseHsts();

    // Configure endpoints based on environment
    Startup.ConfigureEndpoints(app);

    // Configure Swagger generation and TypeScript client in development
    if (env.IsDevelopment())
    {
        app.UseSwaggerGen();
        app.MapTypeScriptClientEndpoint(
            "/ts-client",
            "version 1",
            s =>
            {
                s.ClassName = "ApiClient";
                s.TypeScriptGeneratorSettings.Namespace = "My Namespace";
            }
        );
    }

    // Use WatchDog exception logger and WatchDog UI
    if (!env.IsIntegrationTesting())
    {
        app.UseWatchDogExceptionLogger();
        app.UseWatchDog(opt =>
        {
            opt.WatchPageUsername = "admin";
            opt.WatchPagePassword = "password";
        });
    }

    // Apply database migrations and run the application
    using var scope = app.Services.CreateScope();
    var repository = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await repository.Database.MigrateAsync();
    app.Run();
}
catch (Exception ex)
{
    // Handle startup error and enter maintenance mode
    var builder = WebApplication.CreateBuilder(args);
    builder.Logging.AddDebug().AddConsole().AddAzureWebAppDiagnostics().SetMinimumLevel(LogLevel.Debug);
    var app = builder.Build();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Startup Error -> Entering Maintenance Mode");
    app.MapGet("/{**path}", () => "Maintenance Mode");
    app.Run();
}

public partial class Program { }
