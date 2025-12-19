using System.Security.Claims;
using api.Etc;
using api.Models;
using api.Models.Dtos.Requests;
using api.Models.Dtos.Requests.Auth;
using api.Models.Requests;
using api.Services;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class AuthServiceTests
{
    private sealed class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
    }

    private static TestDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase($"auth-tests-{Guid.NewGuid():N}")
            .EnableSensitiveDataLogging()
            .Options;

        return new TestDbContext(options);
    }

    private readonly Mock<ILogger<AuthService>> _loggerMock = new();
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock = new();
    private readonly Mock<IRepository<User>> _userRepositoryMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();

    private readonly AppOptions _appOptions = new()
    {
        FrontendUrl = "https://frontend.test"
    };

    private AuthService CreateService()
        => new(
            _loggerMock.Object,
            _passwordHasherMock.Object,
            _userRepositoryMock.Object,
            _emailServiceMock.Object,
            _appOptions);

    private static User NewUser(
        string email = "test@test.com",
        string passwordHash = "hashed",
        string name = "Test User",
        string phone = "12345678",
        bool isAdmin = false,
        DateTime? deletedAt = null,
        DateTime? expiresAt = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            Name = name,
            PhoneNumber = phone,          
            IsAdmin = isAdmin,
            DeletedAt = deletedAt,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            PasswordResetToken = null,
            PasswordResetTokenExpiry = null
        };
    }

    private void WireRepositoryToDb(TestDbContext db)
    {
        _userRepositoryMock.Reset();

        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(db.Users);

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(async (User u) =>
            {
                db.Users.Add(u);
                await db.SaveChangesAsync();
            });

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(async (User u) =>
            {
                db.Users.Update(u);
                await db.SaveChangesAsync();
            });
    }

    // ----------------------------
    // Authenticate
    // ----------------------------

    [Fact]
    public void Authenticate_ValidCredentials_ReturnsUserInfo()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var user = NewUser(isAdmin: true);
        db.Users.Add(user);
        db.SaveChanges();

        _passwordHasherMock
            .Setup(h => h.VerifyHashedPassword(user, user.PasswordHash, "password"))
            .Returns(PasswordVerificationResult.Success);

        var service = CreateService();

        var result = service.Authenticate(new LoginRequestDto
        {
            Email = user.Email,
            Password = "password"
        });

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.True(result.IsAdmin);
    }

    [Fact]
    public void Authenticate_InvalidPassword_ThrowsAuthenticationError()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var user = NewUser();
        db.Users.Add(user);
        db.SaveChanges();

        _passwordHasherMock
            .Setup(h => h.VerifyHashedPassword(user, user.PasswordHash, "wrong"))
            .Returns(PasswordVerificationResult.Failed);

        var service = CreateService();

        Assert.ThrowsAny<AuthenticationError>(() =>
            service.Authenticate(new LoginRequestDto
            {
                Email = user.Email,
                Password = "wrong"
            }));
    }

    [Fact]
    public void Authenticate_DeletedUser_ThrowsAuthenticationError()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var user = NewUser(deletedAt: DateTime.UtcNow.AddDays(-1));
        db.Users.Add(user);
        db.SaveChanges();

        var service = CreateService();

        var ex = Assert.ThrowsAny<AuthenticationError>(() =>
            service.Authenticate(new LoginRequestDto
            {
                Email = user.Email,
                Password = "password"
            }));

        Assert.Contains("inactive", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Authenticate_ExpiredMembership_ThrowsAuthenticationError()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var user = NewUser(expiresAt: DateTime.UtcNow.AddMinutes(-1));
        db.Users.Add(user);
        db.SaveChanges();

        var service = CreateService();

        var ex = Assert.ThrowsAny<AuthenticationError>(() =>
            service.Authenticate(new LoginRequestDto
            {
                Email = user.Email,
                Password = "password"
            }));

        Assert.Contains("expired", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    // ----------------------------
    // Register
    // ----------------------------

    [Fact]
    public async Task Register_NewUser_AddsUserAndReturnsInfo()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        _passwordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), "password"))
            .Returns("hashed");

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(async (User u) =>
            {
                u.PhoneNumber = "12345678"; // REQUIRED FIELD
                db.Users.Add(u);
                await db.SaveChangesAsync();
            });

        var service = CreateService();

        var result = await service.Register(new RegisterRequestDto
        {
            Email = "new@test.com",
            Name = "New User",
            Password = "password"
        });

        Assert.Equal("New User", result.Name);
        Assert.False(result.IsAdmin);

        var saved = await db.Users.SingleAsync(u => u.Email == "new@test.com");
        Assert.Equal("12345678", saved.PhoneNumber);

        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }


    [Fact]
    public async Task Register_EmailExists_ThrowsAuthenticationError()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        db.Users.Add(NewUser(email: "test@test.com"));
        await db.SaveChangesAsync();

        var service = CreateService();

        await Assert.ThrowsAnyAsync<AuthenticationError>(() =>
            service.Register(new RegisterRequestDto
            {
                Email = "test@test.com",
                Name = "Test",
                Password = "password"
            }));
    }

    // ----------------------------
    // GetUserInfo
    // ----------------------------

    [Fact]
    public void GetUserInfo_UserExists_ReturnsDto()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var userId = Guid.NewGuid();
        var user = NewUser();
        user.Id = userId;

        db.Users.Add(user);
        db.SaveChanges();

        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }));

        var service = CreateService();

        var result = service.GetUserInfo(claims);

        Assert.NotNull(result);
        Assert.Equal(userId, result!.Id);
    }

    // ----------------------------
    // SendPasswordResetEmail
    // ----------------------------

    [Fact]
    public async Task SendPasswordResetEmail_UserExists_SetsToken_UpdatesUser_AndSendsEmail()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var user = NewUser();
        db.Users.Add(user);
        await db.SaveChangesAsync();

        _emailServiceMock
            .Setup(e => e.SendPasswordResetEmail(user.Email, user.Name, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        await service.SendPasswordResetEmail(user.Email);

        var saved = await db.Users.SingleAsync(u => u.Id == user.Id);

        Assert.False(string.IsNullOrWhiteSpace(saved.PasswordResetToken));
        Assert.NotNull(saved.PasswordResetTokenExpiry);
        Assert.True(saved.PasswordResetTokenExpiry > DateTime.UtcNow);

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmail(user.Email, user.Name, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SendPasswordResetEmail_UserNotFound_DoesNothing()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var service = CreateService();

        await service.SendPasswordResetEmail("missing@test.com");

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    // ----------------------------
    // ResetPassword
    // ----------------------------

    [Fact]
    public async Task ResetPassword_ValidToken_UpdatesPassword_AndClearsToken()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var user = NewUser();
        user.PasswordResetToken = "token";
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        _passwordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), "newpassword"))
            .Returns("new-hash");

        var service = CreateService();

        await service.ResetPassword(new ResetPasswordRequestDto
        {
            Email = user.Email,
            Token = "token",
            NewPassword = "newpassword"
        });

        var saved = await db.Users.SingleAsync(u => u.Id == user.Id);

        Assert.Equal("new-hash", saved.PasswordHash);
        Assert.Null(saved.PasswordResetToken);
        Assert.Null(saved.PasswordResetTokenExpiry);

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ThrowsAuthenticationError()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var user = NewUser();
        user.PasswordResetToken = "correct";
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = CreateService();

        await Assert.ThrowsAnyAsync<AuthenticationError>(() =>
            service.ResetPassword(new ResetPasswordRequestDto
            {
                Email = user.Email,
                Token = "wrong",
                NewPassword = "password"
            }));
    }

    [Fact]
    public async Task ResetPassword_ExpiredToken_ThrowsAuthenticationError()
    {
        using var db = CreateDb();
        WireRepositoryToDb(db);

        var user = NewUser();
        user.PasswordResetToken = "token";
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(-1);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = CreateService();

        await Assert.ThrowsAnyAsync<AuthenticationError>(() =>
            service.ResetPassword(new ResetPasswordRequestDto
            {
                Email = user.Email,
                Token = "token",
                NewPassword = "password"
            }));
    }
}
