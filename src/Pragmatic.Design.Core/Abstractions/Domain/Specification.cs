using System.Linq.Expressions;

namespace Pragmatic.Design.Core.Abstractions.Domain;

public class Specification<T>
    where T : class
{
    public Expression<Func<T, bool>> Criteria { get; }

    public Specification(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
    }

    public bool IsSatisfiedBy(T entity)
    {
        var predicate = Criteria.Compile();
        return predicate(entity);
    }

    public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
    {
        return specification.Criteria;
    }

    public static Specification<T> operator &(Specification<T> left, Specification<T> right)
    {
        var leftExpression = left.Criteria;
        var rightExpression = right.Criteria;

        var parameter = Expression.Parameter(typeof(T));

        var body = Expression.AndAlso(Expression.Invoke(leftExpression, parameter), Expression.Invoke(rightExpression, parameter));

        var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

        return new Specification<T>(lambda);
    }

    public static Specification<T> operator |(Specification<T> left, Specification<T> right)
    {
        var leftExpression = left.Criteria;
        var rightExpression = right.Criteria;

        var parameter = Expression.Parameter(typeof(T));

        var body = Expression.OrElse(Expression.Invoke(leftExpression, parameter), Expression.Invoke(rightExpression, parameter));

        var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

        return new Specification<T>(lambda);
    }

    public static Specification<T> operator !(Specification<T> specification)
    {
        var expression = specification.Criteria;

        var parameter = Expression.Parameter(typeof(T));

        var body = Expression.Not(Expression.Invoke(expression, parameter));

        var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

        return new Specification<T>(lambda);
    }
}
