namespace Pragmatic.Design.DataProcessor.Persistence;

internal interface IDataProcessorTaskStore
{
    public Task<IEnumerable<string>> GetExecuted(TaskType taskType);
    public Task AddExecution(TaskInfo taskInfo);
    public Task SetSucceeded(TaskInfo taskInfo);
    public Task SetFailed(TaskInfo taskInfo, Exception e);
}
