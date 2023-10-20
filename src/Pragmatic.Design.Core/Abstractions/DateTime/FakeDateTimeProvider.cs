namespace Pragmatic.Design.Core.Abstractions.Time;

public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow { get; set; }
    public DateOnly Today { get; set; }
    public TimeOnly Time { get; set; }
}
