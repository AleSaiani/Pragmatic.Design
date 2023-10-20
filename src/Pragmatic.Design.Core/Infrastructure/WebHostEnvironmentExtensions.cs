using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Pragmatic.Design.Core.Infrastructure;

public static class WebHostEnvironmentExtensions
{
    public static bool IsTesting(this IWebHostEnvironment env)
    {
        return env.IsEnvironment("Testing");
    }

    public static bool IsIntegrationTesting(this IWebHostEnvironment env)
    {
        return env.IsEnvironment("IntegrationTesting");
    }
}
