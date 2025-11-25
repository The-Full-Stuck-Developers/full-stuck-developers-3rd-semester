using System.Security.Authentication;
using api.Etc;
using api.Models.Dtos.Requests;
using api.Services;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;
using tests.Helpers;
using Xunit;
using IAuthService = api.Services.IAuthService;

namespace tests.Services;

public class AuthServiceTest
{
    IRepository<User> userRepository = null!;
    IPasswordHasher<User> passwordHasher = null!;
    IAuthService sut = null!;
    
    public AuthServiceTest()
    {
        passwordHasher = new FakePasswordHasher<User>();
        String Hash(string password) => passwordHasher.HashPassword(null!, password);
        userRepository = new InMemoryRespository<User>(
            new List<User>
            {
                new User()
                {
                    Id = Guid.NewGuid(),
                    Name = "User1",
                    Email = "admin@example.com",
                    PasswordHash = Hash("fakepassword"),
                    IsAdmin = true,
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    Name = "User2",
                    Email = "user2@example.com",
                    PasswordHash = Hash("fakepassword"),
                    IsAdmin = false,
                },
            }
        );
        sut = new AuthService(
            new LoggerFactory().CreateLogger<AuthService>(),
            passwordHasher,
            userRepository
        );
    }

    [Fact]
    public void Authenticate_Success()
    {
        var response = sut.Authenticate(new LoginRequest{ Email = "user1@example.com", Password = "fakepassword"});
        Xunit.Assert.Equal("User1", response.Name);
    }

    [Fact]
    public void Authenticate_InvalidEmail()
    {
        Xunit.Assert.Throws<AuthenticationError>(() => sut.Authenticate(new LoginRequest{ Email = "invalid", Password = "fakepassword" }));

    }

    [Fact]
    public void Authenticate_InvalidPassword()
    {
        Xunit.Assert.Throws<AuthenticationError>(() => sut.Authenticate(new LoginRequest{ Email = "user1@example.com", Password = "invalid" }));
    }
}