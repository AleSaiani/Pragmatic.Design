namespace Pragmatic.Design.DataProcessor.Fixture;

public interface IFixture
{
    Task Apply();
    string[] Environments { get; }
}
