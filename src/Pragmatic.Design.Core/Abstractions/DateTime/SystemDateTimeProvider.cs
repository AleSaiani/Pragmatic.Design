namespace Pragmatic.Design.Core.Abstractions.Time;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(UtcNow.Date);
    public TimeOnly Time => TimeOnly.FromDateTime(UtcNow);
}
