using Microsoft.EntityFrameworkCore;

namespace Pragmatic.Design.DataProcessor.Persistence;

class DataProcessorDbContext : DbContext
{
    public DataProcessorDbContext(DbContextOptions<DataProcessorDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dataProcessor");

        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }
}
