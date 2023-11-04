using Microsoft.Extensions.Logging;
using Pragmatic.Design.DataProcessor.Persistence;

namespace Pragmatic.Design.DataProcessor.Seeds;

internal class SeedingService
{
    private readonly ILogger logger;
    private readonly IEnumerable<ISeed> seeds;
    private readonly IDataProcessorTaskStore taskStore;

    public SeedingService(ILogger<SeedingService> logger, IEnumerable<ISeed> seeds, IDataProcessorTaskStore taskStore)
    {
        this.logger = logger;
        this.seeds = seeds;
        this.taskStore = taskStore;
    }

    public async Task Execute(DateTimeOffset dataProcessorJobStartTime)
    {
        var seedsInOrder = SeedingDependencyAttribute.OrderByDependencies(seeds);

        var executedSeeds = await taskStore.GetExecuted(TaskType.Seed);

        foreach (var seed in seedsInOrder)
        {
            var taskInfo = new TaskInfo(seed, dataProcessorJobStartTime);

            if (executedSeeds.Contains(taskInfo.Name))
            {
                continue;
            }

            try
            {
                await taskStore.AddExecution(taskInfo);
                await seed.Seed();
                await taskStore.SetSucceeded(taskInfo);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error executing seed {seed.GetType().Name}");
                await taskStore.SetFailed(taskInfo, ex);
            }
        }
    }
}
