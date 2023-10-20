using Pragmatic.Design.Core.Abstractions.Domain;

namespace Pragmatic.Design.Core.Tests;

public class SpecificationTests
{
    public class User
    {
        public bool IsActive { get; init; }
        public List<string> Roles { get; init; } = null!;
        public int Age { get; init; }
    }

    [Fact]
    public void AndOperator_CombinesSpecificationsCorrectly()
    {
        // Arrange
        var user = new User
        {
            IsActive = true,
            Roles = new List<string> { "Admin" }
        };
        var isActive = new Specification<User>(u => u.IsActive);
        var isAdmin = new Specification<User>(u => u.Roles.Contains("Admin"));

        // Act
        var combinedSpec = isActive & isAdmin;
        var result = combinedSpec.IsSatisfiedBy(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void CanBeUsedInLinqWhereClause()
    {
        // Arrange
        var users = new List<User>
        {
            new User
            {
                IsActive = true,
                Roles = new List<string> { "Admin" }
            },
            new User
            {
                IsActive = false,
                Roles = new List<string> { "User" }
            },
            new User
            {
                IsActive = true,
                Roles = new List<string> { "Admin", "User" }
            },
        }.AsQueryable();
        var spec = new Specification<User>(u => u.IsActive && u.Roles.Contains("Admin"));

        // Act
        var activeAdmins = users.Where(spec);

        // Assert
        Assert.Equal(2, activeAdmins.Count());
    }

    [Fact]
    public void CanBeUsedInLinqWhereClause_WithAndOperator()
    {
        // Arrange
        var users = new List<User>
        {
            new User
            {
                IsActive = true,
                Roles = new List<string> { "Admin" }
            },
            new User
            {
                IsActive = true,
                Roles = new List<string> { "User" }
            },
            new User
            {
                IsActive = false,
                Roles = new List<string> { "Admin", "User" }
            },
        }.AsQueryable();
        var isActive = new Specification<User>(u => u.IsActive);
        var isAdmin = new Specification<User>(u => u.Roles.Contains("Admin"));

        // Act
        var activeAdmins = users.Where(isActive & isAdmin).ToList();

        // Assert
        Assert.Single(activeAdmins);
    }

    [Fact]
    public void CanBeUsedInLinqWhereClause_WithNotOperator()
    {
        // Arrange
        var users = new List<User>
        {
            new User
            {
                IsActive = true,
                Roles = new List<string> { "Admin" }
            },
            new User
            {
                IsActive = false,
                Roles = new List<string> { "User" }
            },
            new User
            {
                IsActive = true,
                Roles = new List<string> { "Admin", "User" }
            },
        }.AsQueryable();
        var isActive = new Specification<User>(u => u.IsActive);

        // Act
        var notActiveUsers = users.Where(!isActive).ToList();

        // Assert
        Assert.Single(notActiveUsers);
    }

    [Fact]
    public void CanBeUsedInLinqWhereClause_WithOrOperator()
    {
        // Arrange
        var users = new List<User>
        {
            new User
            {
                IsActive = true,
                Roles = new List<string> { "Admin" }
            },
            new User
            {
                IsActive = false,
                Roles = new List<string> { "User" }
            },
            new User
            {
                IsActive = true,
                Roles = new List<string> { "Admin", "User" }
            },
        }.AsQueryable();
        var isActive = new Specification<User>(u => u.IsActive);
        var isAdmin = new Specification<User>(u => u.Roles.Contains("Admin"));

        // Act
        var activeOrAdmins = users.Where(isActive | isAdmin).ToList();

        // Assert
        Assert.Equal(2, activeOrAdmins.Count());
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsFalse_WhenEntityDoesNotMatchCriteria()
    {
        // Arrange
        var user = new User
        {
            IsActive = false,
            Roles = new List<string> { "User" }
        };
        var spec = new Specification<User>(u => u.IsActive && u.Roles.Contains("Admin"));

        // Act
        var result = spec.IsSatisfiedBy(user);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsSatisfiedBy_ReturnsTrue_WhenEntityMatchesCriteria()
    {
        // Arrange
        var user = new User
        {
            IsActive = true,
            Roles = new List<string> { "Admin" }
        };
        var spec = new Specification<User>(u => u.IsActive && u.Roles.Contains("Admin"));

        // Act
        var result = spec.IsSatisfiedBy(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WorksCorrectly_WithComplexExpressions()
    {
        // Arrange
        var user = new User
        {
            IsActive = true,
            Roles = new List<string> { "Admin", "User" }
        };
        var spec = new Specification<User>(u => u.IsActive && (u.Roles.Contains("Admin") || u.Roles.Contains("User")));

        // Act
        var result = spec.IsSatisfiedBy(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsSatisfiedBy_WorksCorrectly_WithDifferentDataTypes()
    {
        // Arrange
        var user = new User
        {
            IsActive = true,
            Roles = new List<string> { "Admin" },
            Age = 30
        };
        var spec = new Specification<User>(u => u.IsActive && u.Roles.Contains("Admin") && u.Age > 25);

        // Act
        var result = spec.IsSatisfiedBy(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void NotOperator_InvertsSpecificationCorrectly()
    {
        // Arrange
        var user = new User
        {
            IsActive = false,
            Roles = new List<string> { "User" }
        };
        var isActive = new Specification<User>(u => u.IsActive);

        // Act
        var invertedSpec = !isActive;
        var result = invertedSpec.IsSatisfiedBy(user);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OrOperator_CombinesSpecificationsCorrectly()
    {
        // Arrange
        var user = new User
        {
            IsActive = false,
            Roles = new List<string> { "Admin" }
        };
        var isActive = new Specification<User>(u => u.IsActive);
        var isAdmin = new Specification<User>(u => u.Roles.Contains("Admin"));

        // Act
        var combinedSpec = isActive | isAdmin;
        var result = combinedSpec.IsSatisfiedBy(user);

        // Assert
        Assert.True(result);
    }
}
