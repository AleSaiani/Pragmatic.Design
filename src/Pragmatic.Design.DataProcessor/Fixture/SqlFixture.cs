using Microsoft.EntityFrameworkCore;

namespace Pragmatic.Design.DataProcessor.Fixture;

public abstract class SqlFixture : IFixture
{
    protected readonly DbContext dbContext;
    private readonly DataProcessorSettings _dataProcessorSettings;

    public abstract string[] Environments { get; }
    protected virtual string? FilePathRelativeToFixtureRoot { get; }

    public SqlFixture(DbContext dbContext, DataProcessorSettings dataProcessorSettings)
    {
        this.dbContext = dbContext;
        this._dataProcessorSettings = dataProcessorSettings;
    }

    public async Task Apply()
    {
        if (await HasAlreadyBeenApplied())
        {
            return;
        }
        var filePath = GetFilePath();

        var sql = File.ReadAllText(filePath);
        await dbContext.Database.ExecuteSqlRawAsync(sql);
    }

    protected virtual Task<bool> HasAlreadyBeenApplied()
    {
        return Task.FromResult(false);
    }

    private string GetFilePath()
    {
        if (string.IsNullOrWhiteSpace(_dataProcessorSettings.FixtureRootDir))
        {
            throw new Exception($"{nameof(_dataProcessorSettings.FixtureRootDir)} must be registered in the DI when using SQL fixtures.");
        }

        if (FilePathRelativeToFixtureRoot != null)
        {
            return Path.Combine(_dataProcessorSettings.FixtureRootDir, FilePathRelativeToFixtureRoot);
        }

        var expectedFilename = $"{GetType().Name}.sql";
        var expectedDirectory = _dataProcessorSettings.FixtureRootDir;

        var files = Directory.GetFiles(expectedDirectory, expectedFilename, SearchOption.AllDirectories);

        if (!files.Any())
            throw new Exception($"Unable to find SQL file with name '{expectedFilename}' under '{_dataProcessorSettings.FixtureRootDir}'.");

        if (files.Count() > 1)
            throw new Exception($"Multiple files named '{expectedFilename}' under '{_dataProcessorSettings.FixtureRootDir}' found.");

        return files.Single();
    }
}
