namespace Pragmatic.Design.DataProcessor.Fixture;

public record DataProcessorSettings
{
    public string? FixtureRootDir { get; init; }

    internal DataProcessorSettings() { }

    public DataProcessorSettings(string fixturesRootFolder)
    {
        FixtureRootDir = fixturesRootFolder;
    }
}
