using System.Reflection;

namespace Pragmatic.Design.Core.Infrastructure;

public static class ReflectionHelper
{
    public static void ExecuteStaticInterfaceMethod(this List<Assembly> allAssemblies, Type interfaceType, string methodName, params object[] args)
    {
        var types = allAssemblies.SelectMany(k => k.GetTypes()).Where(t => interfaceType.IsAssignableFrom(t) && t != interfaceType);

        foreach (var type in types)
        {
            var configureMethod = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (configureMethod == null)
                continue;
            configureMethod.Invoke(null, args);
        }
    }
}
