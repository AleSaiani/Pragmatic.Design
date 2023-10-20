using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Pragmatic.Design.Core.Abstractions.Domain;

public abstract class BaseEntity { }

public interface IDomainEntity<T>
{
    T Id { get; set; }
}

public interface IDomainEntity<T, TW> : IDomainEntity<T>
{
    TW Id2 { get; set; }
}

public interface IDomainEntity<T, TW, TU> : IDomainEntity<T, TW>
{
    TU Id3 { get; set; }
}

public static class DbContextExtensions
{
    public static Expression<Func<TEntity, bool>> GetByIdExpression<TEntity, TKey>(this DbContext dbContext, TKey key)
        where TEntity : BaseEntity, IDomainEntity<TKey>
    {
        return GetByIdExpression<TEntity>(dbContext, key!);
    }

    public static Expression<Func<TEntity, bool>> GetByIdExpression<TEntity, TKey, TKey2>(this DbContext dbContext, TKey key, TKey2 key2)
        where TEntity : BaseEntity, IDomainEntity<TKey, TKey2>
    {
        return GetByIdExpression<TEntity>(dbContext, key!, key2!);
    }

    public static Expression<Func<TEntity, bool>> GetByIdExpression<TEntity, TKey, TKey2, TKey3>(this DbContext dbContext, TKey key, TKey2 key2, TKey3 key3)
        where TEntity : BaseEntity, IDomainEntity<TKey, TKey2, TKey3>
    {
        return GetByIdExpression<TEntity>(dbContext, key!, key2!, key3!);
    }

    public static Expression<Func<TEntity, bool>> GetByIdExpression<TEntity>(this DbContext context, params object[] keyValues)
        where TEntity : BaseEntity
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context), "DbContext cannot be null.");

        if (keyValues == null || keyValues.Length == 0)
            throw new ArgumentException("At least one key value must be provided.", nameof(keyValues));

        var entityType = context.Model.FindEntityType(typeof(TEntity));
        if (entityType == null)
            throw new InvalidOperationException($"Entity type {typeof(TEntity).FullName} not found in the DbContext.");
        var primaryKey = entityType.FindPrimaryKey();
        if (primaryKey == null)
            throw new InvalidOperationException($"Primary key not found for entity type {typeof(TEntity).FullName}.");

        var keyProperties = primaryKey.Properties;

        if (keyValues.Length != keyProperties.Count)
            throw new ArgumentException($"Number of provided key values ({keyValues.Length}) does not match the number of key properties ({keyProperties.Count}).");

        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var keyEqualityExpressions = new List<Expression>();

        for (var i = 0; i < keyValues.Length; i++)
        {
            var keyProperty = keyProperties[i] ?? throw new ArgumentNullException("keyProperties[i]");
            var property = Expression.Property(parameter, keyProperty.PropertyInfo!);
            var idValue = Convert.ChangeType(keyValues[i], keyProperty.ClrType);
            var constant = Expression.Constant(idValue, keyProperty.ClrType);
            var equality = Expression.Equal(property, constant);
            keyEqualityExpressions.Add(equality);
        }

        var combinedExpression = keyEqualityExpressions.Aggregate(Expression.AndAlso);

        var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);

        return lambda;
    }
}
