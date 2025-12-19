using api;
using dataccess.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public class PasswordHasherTest : IDisposable
{
    private readonly ServiceProvider _rootProvider;

    public PasswordHasherTest()
    {
        var builder = WebApplication.CreateBuilder();
        Program.ConfigureServices(builder, builder.Configuration);

        _rootProvider = builder.Services.BuildServiceProvider();
    }

    public void Dispose()
    {
        _rootProvider.Dispose();
    }

    private IPasswordHasher<User> GetHasherFromScope()
    {
        var scope = _rootProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
    }

    private static User CreateUser() => new User
    {
        Id = Guid.NewGuid(),
        Email = "test@test.local",
        Name = "Test User",
        PhoneNumber = "12345678",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public void HashAndVerifyPassword()
    {
        using var scope = _rootProvider.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var user = CreateUser();
        var password = "S3cret!1";

        var hash = sut.HashPassword(user, password);
        var result = sut.VerifyHashedPassword(user, hash, password);

        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void VerifyPassword_Fails_WithWrongPassword()
    {
        using var scope = _rootProvider.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var user = CreateUser();
        var password = "S3cret!1";
        var wrongPassword = "WrongPassword123";

        var hash = sut.HashPassword(user, password);
        var result = sut.VerifyHashedPassword(user, hash, wrongPassword);

        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    [Fact]
    public void HashPassword_GeneratesDifferentHashes_ForSamePassword()
    {
        using var scope = _rootProvider.CreateScope();
        var sut = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

        var user = CreateUser();
        var password = "S3cret!1";

        var hash1 = sut.HashPassword(user, password);
        var hash2 = sut.HashPassword(user, password);

        Assert.NotEqual(hash1, hash2); 
        Assert.Equal(PasswordVerificationResult.Success, sut.VerifyHashedPassword(user, hash1, password));
        Assert.Equal(PasswordVerificationResult.Success, sut.VerifyHashedPassword(user, hash2, password));
    }
}
