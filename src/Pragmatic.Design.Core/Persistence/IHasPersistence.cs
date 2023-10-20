using Microsoft.EntityFrameworkCore;

namespace Pragmatic.Design.Core.Persistence;

public interface IHasPersistence
{
    static abstract void ConfigurePersistence(ModelBuilder modelBuilder);
}
