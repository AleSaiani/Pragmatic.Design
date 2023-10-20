using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pragmatic.Design.Core.Abstractions.Domain;

namespace Pragmatic.Design.Core.Persistence;

public class Repository : IRepository
{
    private readonly DbContext _dbContext;

    public Repository(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<T?> SingleOrDefaultBySpecification<T>(Specification<T> specification)
        where T : class
    {
        return await _dbContext.Set<T>().SingleOrDefaultAsync(specification);
    }

    public async Task<T> SingleBySpecification<T>(Specification<T> specification)
        where T : class
    {
        return await _dbContext.Set<T>().SingleAsync(specification);
    }

    public async Task<IEnumerable<T>> Where<T>(Specification<T> specification)
        where T : class
    {
        return await _dbContext.Set<T>().Where(specification).ToListAsync();
    }

    public async Task<long> Count<T>(Specification<T> specification)
        where T : class
    {
        return await _dbContext.Set<T>().CountAsync(specification);
    }

    //public Task<List<TProjection>> Query<T, TProjection>(Query<T> query) where T : class
    //{
    //	return query.ApplyTo(_dbContext.Set<T>().AsNoTracking()).ProjectToType<TProjection>().ToListAsync();
    //}

    public async Task<T> Add<T>(T entity)
        where T : class
    {
        await _dbContext.Set<T>().AddAsync(entity);
        return entity;
    }

    public async Task AddRange<T>(IEnumerable<T> entities)
        where T : class
    {
        await _dbContext.Set<T>().AddRangeAsync(entities);
    }

    public void Update<T>(T entity)
        where T : class
    {
        _dbContext.Set<T>().Update(entity);
    }

    public void Delete<T>(T entity)
        where T : class
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public void DeleteRange<T>(IEnumerable<T> entities)
        where T : class
    {
        _dbContext.Set<T>().RemoveRange(entities);
    }

    public async Task<int> DeleteWhere<T>(Specification<T> specification)
        where T : class
    {
        var entities = await _dbContext.Set<T>().Where(specification.Criteria).ToListAsync();
        _dbContext.Set<T>().RemoveRange(entities);
        return entities.Count;
    }

    public Task<int> SaveChanges(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> UpdateWhere<T>(Expression<Func<T, T>> updateExpression, Specification<T> specification)
        where T : class
    {
        var entities = await _dbContext.Set<T>().Where(specification.Criteria).ToListAsync();
        foreach (var entity in entities)
            _dbContext.Entry(entity).CurrentValues.SetValues(updateExpression.Compile()(entity));

        return entities.Count;
    }
}
