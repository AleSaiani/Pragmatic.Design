using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSwag;
using Pragmatic.Design.Core.Abstractions;
using Pragmatic.Design.Core.Infrastructure;
using Pragmatic.Design.Core.Mappings;
using Pragmatic.Design.Core.Mediator;
using Serilog;

namespace Pragmatic.Design.Core.Bootstrap;

public class Startup
{
    public static void Configure(WebApplicationBuilder builder, bool isRunningInAzureWebApp, Assembly[] allAssemblies)
    {
        var env = builder.Environment;
        ConfigureLogging(builder, env);
        ConfigureCors(builder, env, isRunningInAzureWebApp);
        ConfigureSwagger(builder);
        ConfigureLocalization(builder);
    }

    public static void ConfigureDIDiscovery(WebApplicationBuilder builder, List<Assembly> allAssemblies)
    {
        // Scrutor scan
        builder.Services.AddAdvancedDependencyInjection();
        allAssemblies.ExecuteStaticInterfaceMethod(typeof(IMapster), nameof(IMapster.ConfigureMapster));

        var mapper = new MapperConfiguration(mc =>
        {
            allAssemblies.ExecuteStaticInterfaceMethod(typeof(IAutoMapper), nameof(IAutoMapper.ConfigureAutoMapper), mc);
        }).CreateMapper();
        builder.Services.AddSingleton(mapper);
        builder.Services.AddScoped<Root>();
    }

    public static void ConfigureLocalization(WebApplicationBuilder builder)
    {
        builder.Services.Configure<RequestLocalizationOptions>(options =>
        {
            options.ApplyCurrentCultureToResponseHeaders = true;
            options.AddSupportedCultures("it-IT");
            options.AddSupportedUICultures("it-IT");
            options.SetDefaultCulture("it-IT");
            options.RequestCultureProviders.Clear();
            options.RequestCultureProviders.Add(new AcceptLanguageHeaderRequestCultureProvider());
        });
    }

    public static void ConfigureCors(WebApplicationBuilder builder, IWebHostEnvironment env, bool isRunningInAzureWebApp)
    {
        if (!isRunningInAzureWebApp && !env.IsIntegrationTesting())
        {
            var corsOptions = builder.Configuration.GetSection("CORS").Get<CorsOptions>();

            if (corsOptions != null)
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy(
                        "Default",
                        policy =>
                        {
                            if (corsOptions.AllowSpecificOrigins)
                            {
                                policy.WithOrigins(corsOptions.AllowedOrigins.Split(';'));
                                if (!corsOptions.AllowedOrigins.Contains("*"))
                                {
                                    policy.AllowCredentials();
                                }
                            }
                            else
                            {
                                policy.AllowAnyOrigin();
                            }

                            if (corsOptions.AllowSpecificHeaders)
                                policy.WithHeaders(corsOptions.AllowedHeaders.Split(';'));
                            else
                                policy.AllowAnyHeader().WithExposedHeaders("Content-Disposition");

                            if (corsOptions.AllowSpecificMethods)
                                policy.WithMethods(corsOptions.AllowedMethods.Split(';'));
                            else
                                policy.AllowAnyMethod();
                        }
                    );
                });
        }
    }

    public static void ConfigureLogging(WebApplicationBuilder builder, IWebHostEnvironment env)
    {
        if (!env.IsIntegrationTesting())
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger();
            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.Host.UseSerilog();

            builder.Logging
                .AddConsole()
                .AddAzureWebAppDiagnostics()
                //.AddApplicationInsights() We Use Serilog Sinks
                .AddSerilog();
        }
    }

    public static void ConfigureSwagger(WebApplicationBuilder builder)
    {
        builder.Services.SwaggerDocument(o =>
        {
            o.RemoveEmptyRequestSchema = true;
            o.AutoTagPathSegmentIndex = 0;
            o.EnableJWTBearerAuth = false;
            o.DocumentSettings = s =>
            {
                s.Title = "BM Group - Jada";
                s.Version = "v1";
                s.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());
                s.OperationProcessors.Add(new CamelCaseParametersOperationProcessor());
                s.AddAuth(
                    "Cookie",
                    new OpenApiSecurityScheme
                    {
                        Type = OpenApiSecuritySchemeType.ApiKey,
                        In = OpenApiSecurityApiKeyLocation.Cookie,
                        Name = ".AspNetCore.Cookies",
                    }
                );
            };

            o.ShortSchemaNames = true;
            o.SerializerSettings = s =>
            {
                s.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                s.WriteIndented = builder.Environment.IsDevelopment();
            };
        });
    }

    public static void ConfigureEndpoints(WebApplication app)
    {
        app.UseFastEndpoints(c =>
        {
            c.Endpoints.ShortNames = true;
            c.Binding.ValueParserFor<decimal?>(FastEndpointCustomParsers.DecimalParser);
            c.Binding.ValueParserFor<decimal>(FastEndpointCustomParsers.DecimalParser);
            c.Binding.ValueParserFor<double?>(FastEndpointCustomParsers.DoubleParser);
            c.Binding.ValueParserFor<double>(FastEndpointCustomParsers.DoubleParser);

            c.Endpoints.Configurator = ep => ep.PostProcessors(Order.After, new GlobalPostProcessor());
        });
    }
}
