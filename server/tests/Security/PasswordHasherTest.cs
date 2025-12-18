using api;
using dataccess.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable CS8625, CS8618

public class PasswordHasherTest
{
    private readonly IPasswordHasher<User> _sut;

    public PasswordHasherTest()
    {
        var builder = WebApplication.CreateBuilder();
        Program.ConfigureServices(builder, builder.Configuration);

        var app = builder.Build();

        _sut = app.Services.GetRequiredService<IPasswordHasher<User>>();
        Console.WriteLine($"Using password hasher: {_sut.GetType().Name}");
    }

    [Fact]
    public async Task HashAndVerifyPassword()
    {
        // Arrange
        var password = "S3cret!1";
        
        // Act
        var hash = _sut.HashPassword(null, password);
        var result = _sut.VerifyHashedPassword(null, hash, password);
        
        // Assert
        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public async Task VerifyPassword_Fails_WithWrongPassword()
    {
        // Arrange
        var password = "S3cret!1";
        var wrongPassword = "WrongPassword123";
        
        // Act
        var hash = _sut.HashPassword(null, password);
        var result = _sut.VerifyHashedPassword(null, hash, wrongPassword);
        
        // Assert
        Assert.Equal(PasswordVerificationResult.Failed, result);
    }

    [Fact]
    public async Task HashPassword_GeneratesDifferentHashes_ForSamePassword()
    {
        // Arrange
        var password = "S3cret!1";
        
        // Act
        var hash1 = _sut.HashPassword(null, password);
        var hash2 = _sut.HashPassword(null, password);
        
        // Assert
        Assert.NotEqual(hash1, hash2); // Hashes should be different due to salt
        
        // But both should verify correctly
        Assert.Equal(PasswordVerificationResult.Success, _sut.VerifyHashedPassword(null, hash1, password));
        Assert.Equal(PasswordVerificationResult.Success, _sut.VerifyHashedPassword(null, hash2, password));
    }
}