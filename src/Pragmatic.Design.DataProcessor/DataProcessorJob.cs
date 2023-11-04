using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pragmatic.Design.DataProcessor.Fixture;
using Pragmatic.Design.DataProcessor.Seeds;

namespace Pragmatic.Design.DataProcessor;

public class DataProcessorJob : IHostedService
{
    private readonly ILogger logger;
    private readonly IServiceProvider serviceProvider;

    public DataProcessorJob(ILogger<DataProcessorJob> logger, IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var dataProcessorJobStartTime = DateTimeOffset.UtcNow;
            try
            {
                var seedingService = scope.ServiceProvider.GetRequiredService<SeedingService>();
                await seedingService.Execute(dataProcessorJobStartTime);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Seeding service failed");
            }

            try
            {
                var fixtureService = scope.ServiceProvider.GetRequiredService<FixtureService>();
                await fixtureService.Execute(dataProcessorJobStartTime);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fixture service failed");
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
