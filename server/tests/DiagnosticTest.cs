using dataccess;
using dataccess.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace api.Tests;

public class DiagnosticTest
{
    [Fact]
    public void CanCreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new MyDbContext(options);
        
        Assert.NotNull(db);
        Assert.NotNull(db.Users);
        Assert.NotNull(db.Transactions);
    }

    [Fact]
    public void CanAddUser()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var db = new MyDbContext(options);
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Email = "test@test.com",
            PasswordHash = "hash",
            PhoneNumber = "123",
            IsAdmin = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.SaveChanges();

        Assert.Equal(1, db.Users.Count());
    }
}