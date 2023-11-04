using Microsoft.Extensions.Logging;
using Pragmatic.Design.DataProcessor.Persistence;

namespace Pragmatic.Design.DataProcessor.Seeds;

internal class SeedingService
{
    private readonly ILogger logger;
    private readonly IEnumerable<ISeed> seeds;
    private readonly IDataProcessorTaskStore store;

    public SeedingService(ILogger<SeedingService> logger, IEnumerable<ISeed> seeds, IDataProcessorTaskStore store)
    {
        this.logger = logger;
        this.seeds = seeds;
        this.store = store;
    }

    public async Task Execute(DateTimeOffset dataProcessorJobStartTime)
    {
        var seedsInOrder = SeedingDependencyAttribute.OrderByDependencies(seeds);

        var executedSeeds = await store.GetExecutedTasks(TaskType.Seed);

        foreach (var seed in seedsInOrder)
        {
            var taskInfo = new TaskInfo(seed, dataProcessorJobStartTime);

            if (executedSeeds.Contains(taskInfo.Name))
            {
                continue;
            }

            try
            {
                await store.AddTaskExecution(taskInfo);
                await seed.Seed();
                await store.SetTaskSuccedeed(taskInfo);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing seed {Name}", seed.GetType().Name);
                await store.SetTaskFailed(taskInfo);
            }
        }
    }
}
