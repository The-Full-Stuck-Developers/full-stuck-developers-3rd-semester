using api.Models;
using api.Models.Dtos.Requests.User;
using api.Services;
using dataccess;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using Xunit;

namespace api.Tests.Services;

public class UserServiceTests
{
    private readonly MyDbContext _db;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IRepository<User> _userRepository;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new MyDbContext(options);
        _sieveProcessor = new SieveProcessor(Options.Create(new SieveOptions()));
        _userRepository = new UserRepository(_db);
        _userService = new UserService(_userRepository, _sieveProcessor);
    }


#region GetAllUsers Tests

    [Fact]
    public async Task GetAllUsers_ReturnsPagedResult_WithCorrectData()
    {
        // Arrange
        _db.Users.AddRange(
            new User { Id = Guid.NewGuid(), Name = "User 1", Email = "user1@test.com", DeletedAt = null },
            new User { Id = Guid.NewGuid(), Name = "User 2", Email = "user2@test.com", DeletedAt = null }
        );
        await _db.SaveChangesAsync();

        var sieveModel = new SieveModel { Page = 1, PageSize = 10 };

        // Act
        var result = await _userService.GetAllUsers(sieveModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.PageNumber);
    }

    [Fact]
    public async Task GetAllUsers_ExcludesDeletedUsers()
    {
        // Arrange
        _db.Users.AddRange(
            new User { Id = Guid.NewGuid(), Name = "Active User", Email = "active@test.com", DeletedAt = null },
            new User { Id = Guid.NewGuid(), Name = "Deleted User", Email = "deleted@test.com", DeletedAt = DateTime.UtcNow }
        );
        await _db.SaveChangesAsync();

        var sieveModel = new SieveModel { Page = 1, PageSize = 10 };

        // Act
        var result = await _userService.GetAllUsers(sieveModel);

        // Assert
        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Equal("Active User", result.Items.First().Name);
    }

    #endregion

    #region GetUserById Tests

    [Fact]
    public async Task GetUserById_ReturnsUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Users.Add(new User { Id = userId, Name = "Test User", Email = "test@test.com" });
        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("Test User", result.Name);
    }

    [Fact]
    public async Task GetUserById_ReturnsNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region CreateUser Tests

    [Fact]
    public async Task CreateUser_CreatesNewUser_WithCorrectData()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "newuser@test.com",
            PhoneNumber = "1234567890",
            ActivateMembership = true
        };

        // Act
        var result = await _userService.CreateUser(createUserDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New User", result.Name);
        Assert.Equal("newuser@test.com", result.Email);
        Assert.Equal("1234567890", result.PhoneNumber);
        Assert.Equal(1, await _db.Users.CountAsync(cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task CreateUser_ThrowsException_WhenEmailAlreadyExists()
    {
        // Arrange
        _db.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "existing@test.com",
            DeletedAt = null
        });
        await _db.SaveChangesAsync();

        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "existing@test.com",
            PhoneNumber = "1234567890",
            ActivateMembership = false
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _userService.CreateUser(createUserDto));
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public async Task CreateUser_SetsExpiresAt_WhenActivateMembershipIsTrue()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "newuser@test.com",
            ActivateMembership = true
        };

        // Act
        var result = await _userService.CreateUser(createUserDto);

        // Assert
        Assert.NotNull(result.ExpiresAt);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateUser_DoesNotSetExpiresAt_WhenActivateMembershipIsFalse()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "newuser@test.com",
            ActivateMembership = false
        };

        // Act
        var result = await _userService.CreateUser(createUserDto);

        // Assert
        Assert.Null(result.ExpiresAt);
    }

    #endregion

    #region UpdateUser Tests

    [Fact]
    public async Task UpdateUser_UpdatesUser_WithNewData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Users.Add(new User
        {
            Id = userId,
            Name = "Old Name",
            Email = "old@test.com",
            DeletedAt = null
        });
        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        var updateUserDto = new UpdateUserDto
        {
            Name = "New Name",
            Email = "new@test.com"
        };

        // Act
        var result = await _userService.UpdateUser(userId, updateUserDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("new@test.com", result.Email);
    }

    [Fact]
    public async Task UpdateUser_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateUserDto = new UpdateUserDto { Name = "New Name" };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _userService.UpdateUser(userId, updateUserDto));
    }

    [Fact]
    public async Task UpdateUser_DoesNotUpdateName_WhenNameIsNullOrWhitespace()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Users.Add(new User
        {
            Id = userId,
            Name = "Original Name",
            Email = "test@test.com",
            DeletedAt = null
        });
        await _db.SaveChangesAsync();

        var updateUserDto = new UpdateUserDto { Name = "  ", Email = "new@test.com" };

        // Act
        var result = await _userService.UpdateUser(userId, updateUserDto);

        // Assert
        Assert.Equal("Original Name", result.Name);
        Assert.Equal("new@test.com", result.Email);
    }

    #endregion

    #region DeleteUser Tests

    [Fact]
    public async Task DeleteUser_DeletesUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Users.Add(new User { Id = userId, Name = "Test User", Email = "test@test.com" });
        await _db.SaveChangesAsync();

        // Act
        await _userService.DeleteUser(userId);

        // Assert
        var deletedUser = await _db.Users.FindAsync(new object?[] { userId }, TestContext.Current.CancellationToken);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteUser_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _userService.DeleteUser(userId));
    }

    #endregion

    #region RenewMembership Tests

    [Fact]
    public async Task RenewMembership_ExtendsExpiredMembership_FromNow()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Users.Add(new User
        {
            Id = userId,
            Name = "Test User",
            IsAdmin = false,
            ExpiresAt = DateTime.UtcNow.AddDays(-10),
            DeletedAt = null
        });
        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _userService.RenewMembership(userId);

        // Assert
        Assert.NotNull(result.ExpiresAt);
        Assert.True(result.ExpiresAt > DateTime.UtcNow.AddMonths(11));
    }

    [Fact]
    public async Task RenewMembership_ExtendsActiveMembership_WhenWithinRenewalWindow()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var currentExpiry = DateTime.UtcNow.AddDays(5);
        _db.Users.Add(new User
        {
            Id = userId,
            Name = "Test User",
            IsAdmin = false,
            ExpiresAt = currentExpiry,
            DeletedAt = null
        });
        await _db.SaveChangesAsync();

        // Act
        var result = await _userService.RenewMembership(userId);

        // Assert
        Assert.NotNull(result.ExpiresAt);
        Assert.True(result.ExpiresAt > currentExpiry);
    }

    [Fact]
    public async Task RenewMembership_ThrowsException_WhenTooEarlyToRenew()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Users.Add(new User
        {
            Id = userId,
            Name = "Test User",
            IsAdmin = false,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            DeletedAt = null
        });
        await _db.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _userService.RenewMembership(userId));
        Assert.Contains("within 7 days", exception.Message);
    }

    [Fact]
    public async Task RenewMembership_ThrowsException_WhenUserIsAdmin()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Users.Add(new User
        {
            Id = userId,
            Name = "Admin User",
            IsAdmin = true,
            ExpiresAt = DateTime.UtcNow.AddDays(5),
            DeletedAt = null
        });
        await _db.SaveChangesAsync();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _userService.RenewMembership(userId));
        Assert.Contains("Admin users cannot be renewed", exception.Message);
    }

    [Fact]
    public async Task RenewMembership_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _userService.RenewMembership(userId));
    }

    #endregion

    #region GetUsersCount Tests

    [Fact]
    public async Task GetUsersCount_ReturnsCorrectCount_ExcludingDeletedAndAdmins()
    {
        // Arrange
        _db.Users.AddRange(
            new User { Id = Guid.NewGuid(), IsAdmin = false, DeletedAt = null },
            new User { Id = Guid.NewGuid(), IsAdmin = false, DeletedAt = null },
            new User { Id = Guid.NewGuid(), IsAdmin = true, DeletedAt = null },
            new User { Id = Guid.NewGuid(), IsAdmin = false, DeletedAt = DateTime.UtcNow }
        );
        await _db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await _userService.GetUsersCount();

        // Assert
        Assert.Equal(2, result);
    }

    #endregion

    #region ActivateUser Tests

    [Fact]
    public async Task ActivateUser_SetsIsActiveToTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Users.Add(new User { Id = userId, IsActive = false });
        await _db.SaveChangesAsync();

        // Act
        var result = await _userService.ActivateUser(userId);

        // Assert
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task ActivateUser_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _userService.ActivateUser(userId));
    }

    #endregion

    #region DeactivateUser Tests

    [Fact]
    public async Task DeactivateUser_SetsIsActiveToFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Users.Add(new User { Id = userId, IsActive = true });
        await _db.SaveChangesAsync();

        // Act
        var result = await _userService.DeactivateUser(userId);

        // Assert
        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task DeactivateUser_ThrowsException_WhenUserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _userService.DeactivateUser(userId));
    }

    #endregion
}

// Simple repository implementation for testing
public class UserRepository : IRepository<User>
{
    private readonly MyDbContext _context;

    public UserRepository(MyDbContext context)
    {
        _context = context;
    }

    public Task<User?> GetAsync(Func<User, bool> predicate)
    {
        
        var user = _context.Users
            .AsEnumerable()
            .FirstOrDefault(predicate);

        return Task.FromResult(user);
    }

    public IQueryable<User> Query()
    {
        return _context.Users.AsQueryable();
    }

    public async Task AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User entity)
    {
        _context.Users.Remove(entity);
        await _context.SaveChangesAsync();
    }
}