using Microsoft.EntityFrameworkCore;

namespace Pragmatic.Design.Core.Persistence;

/// <summary>
///     ApplicationDbContext class is a DbContext class, which is used to configure the context automatically by using the
///     ApplyConfigurationsFromAssembly method.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    ///     Constructor of ApplicationDbContext class
    /// </summary>
    /// <param name="options">
    ///     The DbContextOptions parameter is a DbContextOptions object, which is used to configure the context.
    /// </param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    /// <summary>
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddIHasPersistenceConfiguration();
    }
}
