using Pragmatic.Design.Core.Persistence;
using Pragmatic.Design.DataProcessor.Seeds;

namespace Pragmatic.Design.WebApi.Host.Examples.DataProcessor;

public class ExampleSeed : ISeed
{
    private readonly ApplicationDbContext _dbContext;

    public ExampleSeed(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task Seed()
    {
        return Task.CompletedTask;
    }
}
