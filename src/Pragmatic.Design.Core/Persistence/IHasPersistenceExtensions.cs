using Microsoft.EntityFrameworkCore;
using Pragmatic.Design.Core.Infrastructure;

namespace Pragmatic.Design.Core.Persistence;

public static class IHasPersistenceExtensions
{
    /// <summary>Add calling configuration method for instances of IHasPersistence</summary>
    public static ModelBuilder AddIHasPersistenceConfiguration(this ModelBuilder modelBuilder)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        foreach (var assembly in assemblies)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }

        assemblies.ExecuteStaticInterfaceMethod(typeof(IHasPersistence), nameof(IHasPersistence.ConfigurePersistence), modelBuilder);

        return modelBuilder;
    }
}
