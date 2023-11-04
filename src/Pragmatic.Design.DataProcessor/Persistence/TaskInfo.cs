using Pragmatic.Design.DataProcessor.Fixture;
using Pragmatic.Design.DataProcessor.Seeds;

namespace Pragmatic.Design.DataProcessor.Persistence;

record TaskInfo
{
    public string Name { get; init; }
    public TaskType Type { get; init; }
    public DateTimeOffset DataProcessorJobStartTime { get; init; }

    internal TaskInfo(IFixture fixture, DateTimeOffset dataProcessorJobStartTime)
    {
        Name = fixture.GetType().Name;
        Type = TaskType.Fixture;
        DataProcessorJobStartTime = dataProcessorJobStartTime;
    }

    internal TaskInfo(ISeed seed, DateTimeOffset dataProcessorJobStartTime)
    {
        Name = seed.GetType().Name;
        Type = TaskType.Seed;
        DataProcessorJobStartTime = dataProcessorJobStartTime;
    }
}
