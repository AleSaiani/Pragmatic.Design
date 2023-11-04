using Microsoft.EntityFrameworkCore;

namespace Pragmatic.Design.DataProcessor.Persistence;

class DataProcessorTaskStore : IDataProcessorTaskStore
{
    readonly DataProcessorDbContext dbContext;

    public DataProcessorTaskStore(DataProcessorDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IEnumerable<string>> GetExecuted(TaskType taskType)
    {
        var executedTasks = await dbContext
            .Set<TaskExecution>()
            .Where(te => te.State == TaskExecutionState.Succeeded && te.TaskType == taskType)
            .Select(te => te.Name)
            .ToListAsync();
        return executedTasks;
    }

    public async Task AddExecution(TaskInfo taskInfo)
    {
        var execution = new TaskExecution(taskInfo);
        await dbContext.AddAsync(execution);
        await dbContext.SaveChangesAsync();
    }

    public async Task SetSucceeded(TaskInfo taskInfo)
    {
        var execution = await dbContext.Set<TaskExecution>().Where(m => m.DataProcessorJobStartTime == taskInfo.DataProcessorJobStartTime && m.Name == taskInfo.Name && m.TaskType == taskInfo.Type).SingleAsync();
        execution.SetSucceeded();
        await dbContext.SaveChangesAsync();
    }

    public async Task SetFailed(TaskInfo taskInfo, Exception e)
    {
        var execution = await dbContext.Set<TaskExecution>().Where(m => m.DataProcessorJobStartTime == taskInfo.DataProcessorJobStartTime && m.Name == taskInfo.Name && m.TaskType == taskInfo.Type).SingleAsync();
        execution.SetFailed(e);
        await dbContext.SaveChangesAsync();
    }
}
