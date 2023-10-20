using Microsoft.EntityFrameworkCore;

namespace Pragmatic.Design.Core.Tests;

public class QueryTestFixture : IDisposable
{
    public MyDbContext Context { get; }

    public QueryTestFixture()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        Context = new MyDbContext(options);

        Seed();
    }

    public void Dispose()
    {
        Context.Dispose();
    }

    private void Seed()
    {
        Context.Add(
            new Entity
            {
                Id = 1,
                Name = "Entity1",
                RelatedEntity = new RelatedEntity { Id = 1, Description = "RelatedEntity1" }
            }
        );
        Context.Add(
            new Entity
            {
                Id = 2,
                Name = "Entity2",
                RelatedEntity = new RelatedEntity { Id = 2, Description = "RelatedEntity2" }
            }
        );
        Context.SaveChanges();
    }

    public class MyDbContext : DbContext
    {
        public DbSet<Entity> Entities { get; set; } = null!;
        public DbSet<RelatedEntity> RelatedEntities { get; set; } = null!;

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options) { }
    }

    public class Entity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public RelatedEntity RelatedEntity { get; set; } = null!;
    }

    public class RelatedEntity
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
    }

    public class EntityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public RelatedEntityDto RelatedEntity { get; set; } = null!;
    }

    public class RelatedEntityDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
    }
}
