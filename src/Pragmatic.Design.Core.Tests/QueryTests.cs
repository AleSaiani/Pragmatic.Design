//using FluentAssertions;
//using Pragmatic.Design.Core.Abstractions.Domain;
//using Pragmatic.Design.Core.Persistence;
//using static Pragmatic.Design.Core.Tests.QueryTestFixture;

//namespace Pragmatic.Design.Core.Tests;

//public class QueryTests : IClassFixture<QueryTestFixture>
//{
//	public QueryTests(QueryTestFixture fixture)
//	{
//		_fixture = fixture;
//	}

//	private readonly QueryTestFixture _fixture;

//	[Fact]
//	public async Task Execute_ShouldReturnAllEntities()
//	{
//		// Arrange
//		var dbContext = _fixture.Context;
//		var query = new Query<Entity>();

//		// Act
//		var result = await query.Execute<EntityDto>(dbContext.Entities);

//		// Assert
//		result.Should().HaveCount(2);
//		result.Select(e => e.Id).Should().ContainInOrder(1, 2);
//	}

//	[Fact]
//	public async Task Execute_WithInclude_ShouldReturnEntitiesWithIncludedData()
//	{
//		// Arrange
//		var dbContext = _fixture.Context;
//		var query = new Query<Entity>().Include(e => e.RelatedEntity);

//		// Act
//		var result = await query.Execute<EntityDto>(dbContext.Entities);

//		// Assert
//		result.Should().HaveCount(2);
//		result.First().RelatedEntity.Should().NotBeNull();
//	}

//	[Fact]
//	public async Task Execute_WithOrderBy_ShouldReturnOrderedEntities()
//	{
//		// Arrange
//		var dbContext = _fixture.Context;
//		var query = new Query<Entity>().OrderBy(e => e.Name);

//		// Act
//		var result = await query.Execute<EntityDto>(dbContext.Entities);

//		// Assert
//		result.Should().HaveCount(2);
//		result.Select(e => e.Name).Should().ContainInOrder("Entity1", "Entity2");
//	}

//	[Fact]
//	public async Task Execute_WithWhere_ShouldReturnFilteredEntities()
//	{
//		// Arrange
//		var dbContext = _fixture.Context;
//		var specification = new Specification<Entity>(e => e.Name.StartsWith("Entity1"));
//		var query = new Query<Entity>().Where(specification);

//		// Act
//		var result = await query.Execute<EntityDto>(dbContext.Entities);

//		// Assert
//		result.Should().HaveCount(1);
//		result.First().Id.Should().Be(1);
//	}

//	[Fact]
//	public async Task ExecutePaged_ShouldReturnPagedResults()
//	{
//		// Arrange
//		var dbContext = _fixture.Context;
//		var query = new Query<Entity>();
//		var pageIndex = 1;
//		var pageSize = 1;

//		// Act
//		var (data, total) = await query.ExecutePaged<EntityDto>(dbContext.Entities, pageIndex, pageSize);

//		// Assert
//		data.Should().HaveCount(pageSize);
//		total.Should().Be(2);
//	}
//}
