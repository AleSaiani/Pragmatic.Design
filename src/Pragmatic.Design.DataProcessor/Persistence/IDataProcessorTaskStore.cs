using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pragmatic.Design.DataProcessor.Fixture;
using Pragmatic.Design.DataProcessor.Seeds;

namespace Pragmatic.Design.DataProcessor.Persistence;

internal interface IDataProcessorTaskStore
{
    public Task<IEnumerable<string>> GetExecutedTasks(TaskType taskType);
    public Task AddTaskExecution(TaskInfo taskInfo);
    public Task SetTaskSucceded(TaskInfo taskInfo);
    public Task SetTaskFailed(TaskInfo taskInfo);
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

class DapperDataProcessorTaskStore : IDataProcessorTaskStore
{
    //readonly DbContext migrationsDbContext;

    //public DataProcessorTaskStore(DbContext dbContext)
    //{
    //    this.migrationsDbContext = dbContext;
    //}

    //public async Task<IEnumerable<string>> GetExecutedTasks(TaskType taskType)
    //{
    //    var executedTasks = await migrationsDbContext.Set<TaskExecution>().Where(te => te.State == TaskExecutionState.Succeeded).Select(te => te.Name).ToListAsync();
    //    return executedTasks;
    //}

    //public async Task AddTaskExecution(TaskInfo taskInfo)
    //{
    //    var execution = new TaskExecution(taskInfo);
    //    await migrationsDbContext.AddAsync(execution);
    //    await migrationsDbContext.SaveChangesAsync();
    //}

    //public async Task SetTaskSucceeded(TaskInfo taskInfo)
    //{
    //    var execution = await migrationsDbContext
    //        .Set<TaskExecution>()
    //        .Where(m => m.DataProcessorJobStartTime == taskInfo.DataProcessorJobStartTime && m.Name == taskInfo.Name)
    //        .SingleAsync();
    //    execution.SetSucceeded();
    //    await migrationsDbContext.SaveChangesAsync();
    //}

    //public async Task SetTaskFailed(TaskInfo taskInfo, Exception e)
    //{
    //    var execution = await migrationsDbContext
    //        .Set<TaskExecution>()
    //        .Where(m => m.DataProcessorJobStartTime == taskInfo.DataProcessorJobStartTime && m.Name == taskInfo.Name)
    //        .SingleAsync();
    //    execution.SetFailed(e);
    //    await migrationsDbContext.SaveChangesAsync();
    //}

    const string schema = "data";
    const string taskExecutionTable = $"{schema}.{nameof(TaskExecution)}";

    private readonly NpgsqlConnection connection;

    public DapperDataProcessorTaskStore()
    {
        connection = new NpgsqlConnection(CONNECTION_STRING);
        connection.Open();
    }
    public async Task<IEnumerable<string>> GetExecutedTasks(TaskType taskType)
    {
        string sqlCommand = $"SELECT Name FROM {taskExecutionTable} WHERE State = \"{TaskExecutionState.Succeeded}\";";

        return await connection.QueryAsync<string>(sqlCommand);
    }

    public async Task AddTaskExecution(TaskInfo taskInfo)
    {
        string sqlCommand = $"INSERT INTO {taskExecutionTable} (Name, Type, DataProcessorJobStartTime) VALUES (@Name, @Type, @DataProcessorJobStartTime);";

        var queryArgs = new
        {
            taskInfo.Name,
            taskInfo.Type,
            taskInfo.DataProcessorJobStartTime,
        };

        await connection.ExecuteAsync(sqlCommand, queryArgs);
    }

    public async Task SetTaskFailed(TaskInfo taskInfo, Exception e)
    {
        string sqlCommand = $"UPDATE {taskExecutionTable} SET State = @State, EndedAt = @EndedAt, Error = @Error WHERE DataProcessorJobStartTime = @DataProcessorJobStartTime AND Name = @Name;";

        var queryArgs = new
        {
            State = TaskExecutionState.Failed,
            EndedAt = DateTimeOffset.UtcNow,
            Error = e.ToString(),
            taskInfo.DataProcessorJobStartTime,
            taskInfo.Name,
        };

        await connection.ExecuteAsync(sqlCommand, queryArgs);
    }

    public async Task SetTaskSucceded(TaskInfo taskInfo)
    {
        string sqlCommand = $"UPDATE {taskExecutionTable} SET State = @State, EndedAt = @EndedAt WHERE DataProcessorJobStartTime = @DataProcessorJobStartTime AND Name = @Name;";

        var queryArgs = new
        {
            State = TaskExecutionState.Succeeded,
            EndedAt = DateTimeOffset.UtcNow,
            taskInfo.DataProcessorJobStartTime,
            taskInfo.Name,
        };

        await connection.ExecuteAsync(sqlCommand, queryArgs);
    }

    private void EnsureTableExists()
    {
        var sqlCommand = $"CREATE TABLE IF NOT EXISTS {taskExecutionTable} (" +
            $"{nameof(TaskExecution.Id)} varchar(256)" +
            $"{nameof(TaskExecution.TaskType)} varchar(32)" +
            $"{nameof(TaskExecution.Name)} varchar(256)" +
            $"{nameof(TaskExecution.DataProcessorJobStartTime)} timestamptz" +
            $"{nameof(TaskExecution.StartedAt)} timestamptz" +
            $"{nameof(TaskExecution.State)} varchar(32)" +
            $"{nameof(TaskExecution.EndedAt)} timestamptz" +
            $"{nameof(TaskExecution.Error)} text" +
            $" PRIMARY KEY ({nameof(TaskExecution.Id)}))";

        connection.ExecuteAsync(sqlCommand);

    }
}
