using Microsoft.EntityFrameworkCore;
using Pragmatic.Design.DataProcessor.Fixture;
using Pragmatic.Design.DataProcessor.Seeds;

namespace Pragmatic.Design.DataProcessor.Persistence;

internal interface IDataProcessorTaskStore
{
    public Task<IEnumerable<string>> GetExecutedTasks(TaskType taskType);
    public Task AddTaskExecution(TaskInfo taskInfo);
    public Task SetTaskSucceeded(TaskInfo taskInfo);
    public Task SetTaskFailed(TaskInfo taskInfo, Exception e);
}

record TaskInfo
{
    public string Name { get; init; }
    public TaskType Type { get; init; }
    public DateTimeOffset DataProcessorJobStartTime { get; init; }

    internal TaskInfo(IFixture fixture, DateTimeOffset dataProcessorJobStartTime)
    {
        Name = fixture.GetType().Name;
        Type = TaskType.Fixture;
    }

    internal TaskInfo(ISeed seed, DateTimeOffset dataProcessorJobStartTime)
    {
        Name = seed.GetType().Name;
        Type = TaskType.Seed;
    }
}

internal enum TaskType
{
    Fixture,
    Seed
}

class DataProcessorTaskStore
{
    readonly DbContext migrationsDbContext;

    public DataProcessorTaskStore(DbContext dbContext)
    {
        this.migrationsDbContext = dbContext;
    }

    public async Task<IEnumerable<string>> GetExecutedTasks(TaskType taskType)
    {
        var executedTasks = await migrationsDbContext.Set<TaskExecution>().Where(te => te.State == TaskExecutionState.Succeeded).Select(te => te.Name).ToListAsync();
        return executedTasks;
    }

    public async Task AddTaskExecution(TaskInfo taskInfo)
    {
        var execution = new TaskExecution(taskInfo);
        await migrationsDbContext.AddAsync(execution);
        await migrationsDbContext.SaveChangesAsync();
    }

    public async Task SetTaskSucceeded(TaskInfo taskInfo)
    {
        var execution = await migrationsDbContext
            .Set<TaskExecution>()
            .Where(m => m.DataProcessorJobStartTime == taskInfo.DataProcessorJobStartTime && m.Name == taskInfo.Name)
            .SingleAsync();
        execution.SetSucceeded();
        await migrationsDbContext.SaveChangesAsync();
    }

    public async Task SetTaskFailed(TaskInfo taskInfo, Exception e)
    {
        var execution = await migrationsDbContext
            .Set<TaskExecution>()
            .Where(m => m.DataProcessorJobStartTime == taskInfo.DataProcessorJobStartTime && m.Name == taskInfo.Name)
            .SingleAsync();
        execution.SetFailed(e);
        await migrationsDbContext.SaveChangesAsync();
    }
}
