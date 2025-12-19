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
using Microsoft.Extensions.Logging;
using Moq;

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


    [Fact]
    public void Authenticate_ValidCredentials_ReturnsUserInfo()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            Name = "Test User",
            IsAdmin = true,
            PasswordHash = "hashed"
        };

        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(new[] { user }.AsQueryable());

        _passwordHasherMock
            .Setup(h => h.VerifyHashedPassword(user, user.PasswordHash, "password"))
            .Returns(PasswordVerificationResult.Success);

        var service = CreateService();

        var result = service.Authenticate(new LoginRequestDto
        {
            Email = "test@test.com",
            Password = "password"
        });

        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.True(result.IsAdmin);
    }
    
    [Fact]
    public void Authenticate_InvalidPassword_ThrowsAuthenticationError()
    {
        var user = new User
        {
            Email = "test@test.com",
            PasswordHash = "hashed"
        };

        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(new[] { user }.AsQueryable());

        _passwordHasherMock
            .Setup(h => h.VerifyHashedPassword(user, user.PasswordHash, "wrong"))
            .Returns(PasswordVerificationResult.Failed);

        var service = CreateService();

        Assert.Throws<AuthenticationError>(() =>
            service.Authenticate(new LoginRequestDto
            {
                Email = "test@test.com",
                Password = "wrong"
            }));
    }

    [Fact]
    public async Task Register_NewUser_AddsUserAndReturnsInfo()
    {
        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(Enumerable.Empty<User>().AsQueryable());

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
    }

    [Fact]
    public async Task Register_EmailExists_ThrowsAuthenticationError()
    {
        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(new[] { new User { Email = "test@test.com" } }.AsQueryable());

        var service = CreateService();

        await Assert.ThrowsAsync<AuthenticationError>(() =>
            service.Register(new RegisterRequestDto
            {
                Email = "test@test.com",
                Name = "Test",
                Password = "password"
            }));
    }

    [Fact]
    public void GetUserInfo_UserExists_ReturnsDto()
    {
        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            Name = "Test User"
        };

        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(new[] { user }.AsQueryable());

        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }));

        var service = CreateService();

        var result = service.GetUserInfo(claims);

        Assert.NotNull(result);
        Assert.Equal(userId, result!.Id);
    }

    [Fact]
    public async Task SendPasswordResetEmail_UserExists_SendsEmail()
    {
        var user = new User
        {
            Email = "test@test.com",
            Name = "Test User"
        };

        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(new[] { user }.AsQueryable());

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(user))
            .Returns(Task.CompletedTask);

        _emailServiceMock
            .Setup(e => e.SendPasswordResetEmail(
                user.Email,
                user.Name,
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var service = CreateService();

        await service.SendPasswordResetEmail(user.Email);

        Assert.NotNull(user.PasswordResetToken);
        Assert.NotNull(user.PasswordResetTokenExpiry);
    }

    [Fact]
    public async Task ResetPassword_ValidToken_UpdatesPassword()
    {
        var user = new User
        {
            Email = "test@test.com",
            PasswordResetToken = "token",
            PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(10)
        };

        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(new[] { user }.AsQueryable());

        _passwordHasherMock
            .Setup(h => h.HashPassword(user, "newpassword"))
            .Returns("hashed");

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

        Assert.Null(user.PasswordResetToken);
        Assert.Null(user.PasswordResetTokenExpiry);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ThrowsAuthenticationError()
    {
        var user = new User
        {
            Email = "test@test.com",
            PasswordResetToken = "correct",
            PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(10)
        };

        _userRepositoryMock
            .Setup(r => r.Query())
            .Returns(new[] { user }.AsQueryable());

        var service = CreateService();

        await Assert.ThrowsAsync<AuthenticationError>(() =>
            service.ResetPassword(new ResetPasswordRequestDto
            {
                Email = user.Email,
                Token = "wrong",
                NewPassword = "password"
            }));
    }
}

