using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pragmatic.Design.DataProcessor.Fixture;
using Pragmatic.Design.DataProcessor.Persistence;
using Pragmatic.Design.DataProcessor.Seeds;

namespace Pragmatic.Design.DataProcessor;

public static class ConfigureServices
{
    public static IServiceCollection AddDataProcessor(this IServiceCollection services, string connectionString, string? fixtureRootDirectory = null)
    {
        var collection = services
            .AddScoped<SeedingService>()
            .AddScoped<FixtureService>()
            .AddScoped<IDataProcessorTaskStore, DataProcessorTaskStore>()
            .AddSingleton(new DataProcessorSettings() { FixtureRootDir = fixtureRootDirectory })
            .AddHostedService<DataProcessorJob>();

        collection.AddDbContext<DataProcessorDbContext>(o => o.UseNpgsql(connectionString));

        return collection;
    }
}
