using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Pragmatic.Design.DataProcessor.Persistence;

namespace Pragmatic.Design.DataProcessor.Fixture;

internal class FixtureService
{
    private readonly IHostEnvironment hostEnvironment;
    private readonly ILogger<FixtureService> logger;
    private readonly IEnumerable<IFixture> fixtures;
    private readonly IDataProcessorTaskStore store;

    public FixtureService(IHostEnvironment hostEnvironment, ILogger<FixtureService> logger, IEnumerable<IFixture> fixtures, IDataProcessorTaskStore store)
    {
        this.hostEnvironment = hostEnvironment;
        this.logger = logger;
        this.fixtures = fixtures;
        this.store = store;
    }

    public async Task Execute(DateTimeOffset dataProcessorJobStartTime)
    {
        var fixturesInOrder = SeedingDependencyAttribute.OrderByDependencies(fixtures);

        var executedFixtures = await store.GetExecutedTasks(TaskType.Fixture);

        foreach (var fixture in fixturesInOrder)
        {
            var taskInfo = new TaskInfo(fixture, dataProcessorJobStartTime);

            // Skip if fixture is not for this environment or if it was already executed
            if (fixture.Environments.Contains(hostEnvironment.EnvironmentName) || executedFixtures.Contains(taskInfo.Name))
            {
                continue;
            }

            try
            {
                await store.AddTaskExecution(taskInfo);
                await fixture.Apply();
                await store.SetTaskSuccedeed(taskInfo);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error executing fixture {Name}", fixture.GetType().Name);
                await store.SetTaskFailed(taskInfo);
            }
        }
    }
}
