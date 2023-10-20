//namespace Pragmatic.Design.Core.Persistence;

//public class Query<T> where T : class
//{
//	private bool _defined;
//	private Specification<T>? Specification { get; set; }

//	private List<(Expression<Func<T, object>> KeySelector, bool Descending)> OrderByExpressions { get; } =
//		new();

//	private List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

//	public Query<T> Where(Specification<T> specification)
//	{
//		Specification = specification;
//		return this;
//	}

//	protected virtual void Define()
//	{
//		if (_defined)
//			return;
//		_defined = true;
//	}

//	public Query<T> OrderBy(Expression<Func<T, object>> keySelector, bool descending = false)
//	{
//		OrderByExpressions.Add((keySelector, descending));
//		return this;
//	}

//	public Query<T> Include(Expression<Func<T, object>> include)
//	{
//		Includes.Add(include);
//		return this;
//	}

//	private void IncludeFor<TDto>()
//	{
//		var dtoProperties = typeof(TDto).GetProperties();
//		foreach (var dtoProperty in dtoProperties)
//		{
//			var navigationProperty = typeof(T).GetProperty(dtoProperty.Name);
//			if (navigationProperty == null)
//				throw new ArgumentException($"Property {dtoProperty.Name} does not exist on type {typeof(T).Name}");

//			var propertyType = navigationProperty.PropertyType;

//			// Check if the property is a reference type (excluding string) or a collection of reference types
//			var isReferenceType = propertyType.IsClass && propertyType != typeof(string);
//			var isCollectionOfReferenceType = propertyType.IsGenericType &&
//											  typeof(IEnumerable<>).IsAssignableFrom(propertyType
//												  .GetGenericTypeDefinition()) &&
//											  propertyType.GetGenericArguments()[0].IsClass;

//			if (!isReferenceType && !isCollectionOfReferenceType) continue;
//			var parameter = Expression.Parameter(typeof(T), "x");
//			var property = Expression.Property(parameter, navigationProperty);
//			var lambda = Expression.Lambda<Func<T, object>>(property, parameter);
//			Includes.Add(lambda);
//		}
//	}

//	public IQueryable<T> ApplyTo(IQueryable<T> source)
//	{
//		Define();
//		if (Specification != null) source = source.Where(Specification.Criteria);

//		var firstOrderApplied = false;

//		foreach (var (keySelector, descending) in OrderByExpressions)
//			if (!firstOrderApplied)
//			{
//				source = descending ? source.OrderByDescending(keySelector) : source.OrderBy(keySelector);
//				firstOrderApplied = true;
//			}
//			else
//			{
//				source = descending
//							 ? ((IOrderedQueryable<T>)source).ThenByDescending(keySelector)
//							 : ((IOrderedQueryable<T>)source).ThenBy(keySelector);
//			}

//		return Includes.Aggregate(source, (current, include) => current.Include(include));
//	}

//	public async Task<IEnumerable<TDto>> Execute<TDto>(IQueryable<T> source)
//	{
//		IncludeFor<TDto>();
//		return await ApplyTo(source).ProjectToType<TDto>().ToListAsync();
//	}

//	public async Task<(IEnumerable<TDto> Data, int Total)> ExecutePaged<TDto>(
//		IQueryable<T> source, int page, int pageSize)
//	{
//		var data = await ApplyTo(source).Skip((page - 1) * pageSize)
//										.Take(pageSize).ProjectToType<TDto>().ToListAsync();
//		var total = await ApplyTo(source).CountAsync();
//		return (data, total);
//	}
//}
