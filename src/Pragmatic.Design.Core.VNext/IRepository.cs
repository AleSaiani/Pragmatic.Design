using System.Linq.Expressions;
using Pragmatic.Design.Core.Abstractions.Domain;

namespace Pragmatic.Design.Core.Persistence;

public interface IRepository
{
    Task<T?> SingleOrDefaultBySpecification<T>(Specification<T> specification)
        where T : class;
    Task<T> SingleBySpecification<T>(Specification<T> specification)
        where T : class;
    Task<IEnumerable<T>> Where<T>(Specification<T> specification)
        where T : class;
    Task<long> Count<T>(Specification<T> specification)
        where T : class;

    //Task<List<TProjection>> Query<T, TProjection>(Query<T> query) where T : class;
    Task<T> Add<T>(T entity)
        where T : class;
    Task AddRange<T>(IEnumerable<T> entities)
        where T : class;
    void Update<T>(T entity)
        where T : class;
    void Delete<T>(T entity)
        where T : class;
    void DeleteRange<T>(IEnumerable<T> entities)
        where T : class;
    Task<int> DeleteWhere<T>(Specification<T> specification)
        where T : class;
    Task<int> SaveChanges(CancellationToken cancellationToken = default);

    Task<int> UpdateWhere<T>(Expression<Func<T, T>> updateExpression, Specification<T> specification)
        where T : class;
}
