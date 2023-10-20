namespace Pragmatic.Design.Core;

[AttributeUsage(AttributeTargets.Class)]
public class RetryPolicyAttribute : Attribute
{
    public int RetryCount { get; set; } = 3;
    public double ExponentialBackoffFactor { get; set; } = 2;

    public RetryPolicyAttribute(int retryCount, double exponentialBackoffFactor)
    {
        RetryCount = retryCount;
        ExponentialBackoffFactor = exponentialBackoffFactor;
    }
}
