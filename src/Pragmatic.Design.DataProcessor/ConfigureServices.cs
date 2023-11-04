using Microsoft.Extensions.DependencyInjection;
using Pragmatic.Design.DataProcessor.Fixture;
using Pragmatic.Design.DataProcessor.Seeds;

namespace Pragmatic.Design.DataProcessor;

public static class ConfigureServices
{
    public static IServiceCollection AddDataProcessor(this IServiceCollection services, string? fixtureRootDirectory = null)
    {
        return services
            .AddScoped<SeedingService>()
            .AddScoped<FixtureService>()
            .AddSingleton(new DataProcessorSettings() { FixtureRootDir = fixtureRootDirectory })
            .AddHostedService<DataProcessorJob>();
    }
}
