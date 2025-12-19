global using Xunit;

using api.Models.Dtos.Requests.User;
using api.Services;
using dataccess;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace tests.Services;

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

        _sieveProcessor = new SieveProcessor(new SieveOptionsAccessor());
        _userRepository = new UserRepository(_db);
        _userService = new UserService(_userRepository, _sieveProcessor);
    }

    private static User NewUser(
        string name = "Test User",
        string email = "test@test.local",
        string phone = "12345678",
        string passwordHash = "hashed",
        bool isAdmin = false,
        DateTime? deletedAt = null,
        DateTime? expiresAt = null,
        bool isActive = true)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            PhoneNumber = phone,
            PasswordHash = passwordHash,
            IsAdmin = isAdmin,
            DeletedAt = deletedAt,
            ExpiresAt = expiresAt,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static SieveModel DefaultSieve() => new() { Page = 1, PageSize = 50 };

    #region GetAllUsers Tests

    [Fact]
    public async Task GetAllUsers_ReturnsPagedResult_WithCorrectData()
    {
        _db.Users.AddRange(
            NewUser(name: "User 1", email: "user1@test.local"),
            NewUser(name: "User 2", email: "user2@test.local")
        );
        await _db.SaveChangesAsync();

        var result = await _userService.GetAllUsers(DefaultSieve());

        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(50, result.PageSize);
        Assert.Equal(1, result.PageNumber);
    }

    [Fact]
    public async Task GetAllUsers_ExcludesDeletedUsers()
    {
        _db.Users.AddRange(
            NewUser(name: "Active User", email: "active@test.local", deletedAt: null),
            NewUser(name: "Deleted User", email: "deleted@test.local", deletedAt: DateTime.UtcNow)
        );
        await _db.SaveChangesAsync();

        var result = await _userService.GetAllUsers(DefaultSieve());

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Equal("Active User", result.Items.First().Name);
    }

    #endregion

    #region GetUserById Tests

    [Fact]
    public async Task GetUserById_ReturnsUser_WhenUserExists()
    {
        var user = NewUser(name: "Test User", email: "test@test.local");
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _userService.GetUserById(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result!.Id);
        Assert.Equal("Test User", result.Name);
    }

    [Fact]
    public async Task GetUserById_ReturnsNull_WhenUserDoesNotExist()
    {
        var result = await _userService.GetUserById(Guid.NewGuid());
        Assert.Null(result);
    }

    #endregion

    #region CreateUser Tests

    [Fact]
    public async Task CreateUser_CreatesNewUser_WithCorrectData()
    {
        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "newuser@test.local",
            PhoneNumber = "1234567890",
            ActivateMembership = true
        };

        var result = await _userService.CreateUser(createUserDto);

        Assert.NotNull(result);
        Assert.Equal("New User", result.Name);
        Assert.Equal("newuser@test.local", result.Email);
        Assert.Equal("1234567890", result.PhoneNumber);
        Assert.Equal(1, await _db.Users.CountAsync());
    }

    [Fact]
    public async Task CreateUser_ThrowsException_WhenEmailAlreadyExists()
    {
        _db.Users.Add(NewUser(email: "existing@test.local", deletedAt: null));
        await _db.SaveChangesAsync();

        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "existing@test.local",
            PhoneNumber = "1234567890",
            ActivateMembership = false
        };

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.CreateUser(createUserDto));
        Assert.Contains("exists", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateUser_SetsExpiresAt_WhenActivateMembershipIsTrue()
    {
        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "newuser2@test.local",
            PhoneNumber = "1234567890",
            ActivateMembership = true
        };

        var result = await _userService.CreateUser(createUserDto);

        Assert.NotNull(result.ExpiresAt);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task CreateUser_DoesNotSetExpiresAt_WhenActivateMembershipIsFalse()
    {
        var createUserDto = new CreateUserDto
        {
            Name = "New User",
            Email = "newuser3@test.local",
            PhoneNumber = "1234567890",
            ActivateMembership = false
        };

        var result = await _userService.CreateUser(createUserDto);

        Assert.Null(result.ExpiresAt);
    }

    #endregion

    #region UpdateUser Tests

    [Fact]
    public async Task UpdateUser_UpdatesUser_WithNewData()
    {
        var user = NewUser(name: "Old Name", email: "old@test.local", deletedAt: null);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var updateUserDto = new UpdateUserDto
        {
            Name = "New Name",
            Email = "new@test.local"
        };

        var result = await _userService.UpdateUser(user.Id, updateUserDto);

        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal("new@test.local", result.Email);
    }

    [Fact]
    public async Task UpdateUser_ThrowsException_WhenUserNotFound()
    {
        var updateUserDto = new UpdateUserDto { Name = "New Name" };

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdateUser(Guid.NewGuid(), updateUserDto));
    }

    [Fact]
    public async Task UpdateUser_DoesNotUpdateName_WhenNameIsNullOrWhitespace()
    {
        var user = NewUser(name: "Original Name", email: "test@test.local", deletedAt: null);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var updateUserDto = new UpdateUserDto { Name = "  ", Email = "new@test.local" };

        var result = await _userService.UpdateUser(user.Id, updateUserDto);

        Assert.Equal("Original Name", result.Name);
        Assert.Equal("new@test.local", result.Email);
    }

    #endregion

    #region DeleteUser Tests

    [Fact]
    public async Task DeleteUser_DeletesUser_WhenUserExists()
    {
        var user = NewUser(name: "Test User", email: "test@test.local");
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await _userService.DeleteUser(user.Id);

        var deletedUser = await _db.Users.FindAsync(user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteUser_ThrowsException_WhenUserNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.DeleteUser(Guid.NewGuid()));
    }

    #endregion

    #region RenewMembership Tests

    [Fact]
    public async Task RenewMembership_ExtendsExpiredMembership_FromNow()
    {
        var user = NewUser(
            name: "Test User",
            email: "renew1@test.local",
            isAdmin: false,
            expiresAt: DateTime.UtcNow.AddDays(-10),
            deletedAt: null
        );
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _userService.RenewMembership(user.Id);

        Assert.NotNull(result.ExpiresAt);

        // loose bounds to avoid flakiness (service likely sets +1 year)
        Assert.True(result.ExpiresAt > DateTime.UtcNow.AddMonths(11));
        Assert.True(result.ExpiresAt < DateTime.UtcNow.AddMonths(13));
    }

    [Fact]
    public async Task RenewMembership_ExtendsActiveMembership_WhenWithinRenewalWindow()
    {
        var currentExpiry = DateTime.UtcNow.AddDays(5);
        var user = NewUser(
            name: "Test User",
            email: "renew2@test.local",
            isAdmin: false,
            expiresAt: currentExpiry,
            deletedAt: null
        );
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _userService.RenewMembership(user.Id);

        Assert.NotNull(result.ExpiresAt);
        Assert.True(result.ExpiresAt > currentExpiry);
    }

    [Fact]
    public async Task RenewMembership_ThrowsException_WhenTooEarlyToRenew()
    {
        var user = NewUser(
            name: "Test User",
            email: "renew3@test.local",
            isAdmin: false,
            expiresAt: DateTime.UtcNow.AddDays(30),
            deletedAt: null
        );
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RenewMembership(user.Id));
        Assert.Contains("within 7 days", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RenewMembership_ThrowsException_WhenUserIsAdmin()
    {
        var user = NewUser(
            name: "Admin User",
            email: "renew4@test.local",
            isAdmin: true,
            expiresAt: DateTime.UtcNow.AddDays(5),
            deletedAt: null
        );
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.RenewMembership(user.Id));
        Assert.Contains("admin", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task RenewMembership_ThrowsException_WhenUserNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.RenewMembership(Guid.NewGuid()));
    }

    #endregion

    #region GetUsersCount Tests

    [Fact]
    public async Task GetUsersCount_ReturnsCorrectCount_ExcludingDeletedAndAdmins()
    {
        _db.Users.AddRange(
            NewUser(email: "c1@test.local", isAdmin: false, deletedAt: null),
            NewUser(email: "c2@test.local", isAdmin: false, deletedAt: null),
            NewUser(email: "admin@test.local", isAdmin: true, deletedAt: null),
            NewUser(email: "deleted@test.local", isAdmin: false, deletedAt: DateTime.UtcNow)
        );
        await _db.SaveChangesAsync();

        var result = await _userService.GetUsersCount();

        Assert.Equal(2, result);
    }

    #endregion

    #region ActivateUser Tests

    [Fact]
    public async Task ActivateUser_SetsIsActiveToTrue()
    {
        var user = NewUser(email: "act@test.local", isActive: false);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _userService.ActivateUser(user.Id);

        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task ActivateUser_ThrowsException_WhenUserNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.ActivateUser(Guid.NewGuid()));
    }

    #endregion

    #region DeactivateUser Tests

    [Fact]
    public async Task DeactivateUser_SetsIsActiveToFalse()
    {
        var user = NewUser(email: "deact@test.local", isActive: true);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _userService.DeactivateUser(user.Id);

        Assert.False(result.IsActive);
    }

    [Fact]
    public async Task DeactivateUser_ThrowsException_WhenUserNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.DeactivateUser(Guid.NewGuid()));
    }

    #endregion
}

public class UserRepository : IRepository<User>
{
    private readonly MyDbContext _context;

    public UserRepository(MyDbContext context) => _context = context;

    public Task<User?> GetAsync(Func<User, bool> predicate)
    {
        var user = _context.Users.AsEnumerable().FirstOrDefault(predicate);
        return Task.FromResult(user);
    }

    public IQueryable<User> Query() => _context.Users.AsQueryable();

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

public class SieveOptionsAccessor : Microsoft.Extensions.Options.IOptions<SieveOptions>
{
    public SieveOptions Value => new()
    {
        DefaultPageSize = 50,
        MaxPageSize = 200
    };
}
