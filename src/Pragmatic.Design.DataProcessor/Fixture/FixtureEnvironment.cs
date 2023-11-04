using Microsoft.Extensions.Hosting;

namespace Pragmatic.Design.DataProcessor.Fixture;

public static class FixtureEnvironment
{
    public static readonly string Development = Environments.Development;
    public static readonly string Testing = "Testing";
    public static readonly string Staging = Environments.Staging;

    public static readonly string[] All = new[] { Development, Testing, Staging };
}
