using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
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
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class AuthServiceTests
{
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
            IsActive = true
        };
    }

    private void SetupUsers(params User[] users)
    {
        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(users.AsAsyncQueryable());
    }

    // ----------------------------
    // Authenticate
    // ----------------------------

    [Fact]
    public void Authenticate_ValidCredentials_ReturnsUserInfo()
    {
        var user = NewUser(isAdmin: true);

        SetupUsers(user);

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
        var user = NewUser();

        SetupUsers(user);

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
        var user = NewUser(deletedAt: DateTime.UtcNow.AddDays(-1));
        SetupUsers(user);

        var service = CreateService();

        Assert.ThrowsAny<AuthenticationError>(() =>
            service.Authenticate(new LoginRequestDto
            {
                Email = user.Email,
                Password = "password"
            }));
    }

    [Fact]
    public void Authenticate_ExpiredMembership_ThrowsAuthenticationError()
    {
        var user = NewUser(expiresAt: DateTime.UtcNow.AddMinutes(-1));
        SetupUsers(user);

        var service = CreateService();

        Assert.ThrowsAny<AuthenticationError>(() =>
            service.Authenticate(new LoginRequestDto
            {
                Email = user.Email,
                Password = "password"
            }));
    }

    // ----------------------------
    // Register
    // ----------------------------

    [Fact]
    public async Task Register_NewUser_AddsUserAndReturnsInfo()
    {
        SetupUsers(); // empty list

        _passwordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), "password"))
            .Returns("hashed");

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        var result = await service.Register(new RegisterRequestDto
        {
            Email = "new@test.com",
            Name = "New User",
            Password = "password"
        });

        Assert.Equal("New User", result.Name);
        Assert.False(result.IsAdmin);

        _userRepositoryMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == "new@test.com")), Times.Once);
    }

    [Fact]
    public async Task Register_EmailExists_ThrowsAuthenticationError()
    {
        SetupUsers(NewUser(email: "test@test.com"));

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
        var userId = Guid.NewGuid();
        var user = NewUser();
        user.Id = userId;

        SetupUsers(user);

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
        var user = NewUser();
        SetupUsers(user);

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _emailServiceMock
            .Setup(e => e.SendPasswordResetEmail(user.Email, user.Name, It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        await service.SendPasswordResetEmail(user.Email);

        Assert.False(string.IsNullOrWhiteSpace(user.PasswordResetToken));
        Assert.NotNull(user.PasswordResetTokenExpiry);

        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        _emailServiceMock.Verify(e => e.SendPasswordResetEmail(user.Email, user.Name, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task SendPasswordResetEmail_UserNotFound_DoesNothing()
    {
        SetupUsers(); // empty

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
        var user = NewUser();
        user.PasswordResetToken = "token";
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        SetupUsers(user);

        _passwordHasherMock
            .Setup(h => h.HashPassword(user, "newpassword"))
            .Returns("new-hash");

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(user))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        await service.ResetPassword(new ResetPasswordRequestDto
        {
            Email = user.Email,
            Token = "token",
            NewPassword = "newpassword"
        });

        Assert.Equal("new-hash", user.PasswordHash);
        Assert.Null(user.PasswordResetToken);
        Assert.Null(user.PasswordResetTokenExpiry);

        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ThrowsAuthenticationError()
    {
        var user = NewUser();
        user.PasswordResetToken = "correct";
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);

        SetupUsers(user);

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
        var user = NewUser();
        user.PasswordResetToken = "token";
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(-1);

        SetupUsers(user);

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

// ------------------------------------------------------------------
// Async IQueryable that supports EF Core's FirstOrDefaultAsync, etc.
// ------------------------------------------------------------------

internal static class AsyncQueryableExtensions
{
    public static IQueryable<T> AsAsyncQueryable<T>(this IEnumerable<T> source)
        => new TestAsyncEnumerable<T>(source);
}

internal sealed class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
    public TestAsyncEnumerable(Expression expression) : base(expression) { }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

    IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal sealed class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    private readonly IEnumerator<T> _inner;

    public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

    public T Current => _inner.Current;

    public ValueTask DisposeAsync()
    {
        _inner.Dispose();
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
        => new(_inner.MoveNext());
}

internal sealed class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
    private readonly IQueryProvider _inner;

    public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

    public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);

    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    public object Execute(Expression expression) => _inner.Execute(expression)!;

    public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression)!;

    // EF Core calls this with TResult = Task<T>
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        // unwrap Task<T>
        var tResult = typeof(TResult);
        if (tResult.IsGenericType && tResult.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var innerType = tResult.GetGenericArguments()[0];
            var execGeneric = typeof(IQueryProvider).GetMethods()
                .Single(m => m.Name == nameof(IQueryProvider.Execute) && m.IsGenericMethod && m.GetParameters().Length == 1)
                .MakeGenericMethod(innerType);

            var result = execGeneric.Invoke(_inner, new object[] { expression });

            var fromResult = typeof(Task).GetMethods()
                .Single(m => m.Name == nameof(Task.FromResult) && m.IsGenericMethod)
                .MakeGenericMethod(innerType);

            return (TResult)fromResult.Invoke(null, new[] { result })!;
        }

        // If EF calls with a non-Task result, just execute sync
        return Execute<TResult>(expression);
    }

    public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        => new TestAsyncEnumerable<TResult>(expression);
}
