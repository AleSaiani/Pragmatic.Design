namespace Pragmatic.Design.Core.Abstractions.Time;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
    TimeOnly Time { get; }
}
