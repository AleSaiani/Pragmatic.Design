using System.Reflection;

namespace Pragmatic.Design.DataProcessor;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SeedingDependencyAttribute : Attribute
{
    public Type[] DependsOnType { get; private set; }

    public SeedingDependencyAttribute(params Type[] dependsOnType)
    {
        DependsOnType = dependsOnType;
    }

    public static List<T> OrderByDependencies<T>(IEnumerable<T> types)
        where T : notnull
    {
        var orderedTypes = new List<T>();

        var typeGraph = types.ToDictionary(t => t, t => t.GetType().GetCustomAttributes<SeedingDependencyAttribute>().SelectMany(attr => attr.DependsOnType).ToList());

        while (typeGraph.Count > 0)
        {
            var candidates = typeGraph.Where(kv => kv.Value.All(orderedTypes.Select(t => t.GetType()).Contains)).ToList();
            if (!candidates.Any())
                throw new InvalidOperationException("Cannot resolve seed dependency order, check for circular dependencies or dependencies with mismatching environments.");

            foreach (var candidate in candidates)
            {
                orderedTypes.Add(candidate.Key);
                typeGraph.Remove(candidate.Key);
            }
        }

        return orderedTypes;
    }
}
