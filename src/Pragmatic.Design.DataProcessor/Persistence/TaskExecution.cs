namespace Pragmatic.Design.DataProcessor.Persistence;

class TaskExecution
{
    public string Id { get; private init; }
    public TaskType TaskType { get; private init; }
    public string Name { get; private init; }
    public DateTimeOffset DataProcessorJobStartTime { get; private init; }
    public DateTimeOffset StartedAt { get; private init; }
    public TaskExecutionState State { get; private set; }
    public DateTimeOffset? EndedAt { get; private set; }
    public string? Error { get; private set; }

    protected TaskExecution() { }

    public TaskExecution(TaskInfo taskInfo)
    {
        Id = Guid.NewGuid().ToString();
        TaskType = taskInfo.Type;
        Name = taskInfo.Name;
        DataProcessorJobStartTime = taskInfo.DataProcessorJobStartTime;
        State = TaskExecutionState.Started;
        StartedAt = DateTimeOffset.UtcNow;
    }

    internal void SetSucceeded()
    {
        if (State != TaskExecutionState.Started)
        {
            throw new InvalidOperationException("Unexpeceted TaskExecution State: " + State.ToString());
        }

        State = TaskExecutionState.Succeeded;
        EndedAt = DateTimeOffset.UtcNow;
    }

    internal void SetFailed(Exception e)
    {
        if (State != TaskExecutionState.Started)
        {
            throw new InvalidOperationException("Unexpeceted TaskExecution State: " + State.ToString());
        }

        State = TaskExecutionState.Failed;
        EndedAt = DateTimeOffset.UtcNow;
        Error = e.ToString();
    }
}

enum TaskExecutionState
{
    Started,
    Failed,
    Succeeded
}
