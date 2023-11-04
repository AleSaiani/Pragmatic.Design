using Pragmatic.Design.Core.Persistence;
using Pragmatic.Design.DataProcessor.Fixture;

namespace Pragmatic.Design.WebApi.Host.Examples.DataProcessor;

public class ExampleSqlFixture : SqlFixture
{
    private readonly ApplicationDbContext _dbContext;
    public override string[] Environments => new[] { FixtureEnvironment.Development };

    public ExampleSqlFixture(ApplicationDbContext dbContext, DataProcessorSettings dataProcessorSettings)
        : base(dbContext, dataProcessorSettings)
    {
        _dbContext = dbContext;
    }
}
