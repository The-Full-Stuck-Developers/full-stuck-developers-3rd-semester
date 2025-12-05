using api.Etc;
using dataccess;
using dataccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Etc;

public class DbSeeder(
    MyDbContext context,
    IPasswordHasher<User> hasher
) 

{
    public async Task Seed(String defaultPassword)
    {
        // 1. Ensure Database Exists
        context.Database.EnsureCreated();
        if (!await context.Users.AnyAsync())
        {

            await CreateUsers(
                [
                    (email: "admin@example.com", name: "Jesus", phoneNumber:"55225522", isAdmin: true),
                    (email: "user@example.com", "Fred", "53998832", isAdmin: false),
                    (email: "player@example.com", "Peter", "912043984", isAdmin: false),
                ],
                defaultPassword
            );
        }

        if (!await context.Games.AnyAsync())
        {
            context.Games.Add(
                new Game
                {
                    StartTime = DateTime.UtcNow,
                    // IsActive = true,
                    Revenue = 0,
                    WinningNumbers = null
                });
            await context.SaveChangesAsync();
        }
    }
    private async Task CreateUsers((string email, string name,string phoneNumber, bool isAdmin)[] users, string defaultPassword)
    {
        foreach (var userData in users)
        {
            var user = new User
                {
                    Email = userData.email,
                    Name = userData.name,
                    PhoneNumber = userData.phoneNumber,
                    IsAdmin = userData.isAdmin,
                    Balance = 124500,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    PasswordHash = hasher.HashPassword(null, defaultPassword),
                };
                
                context.Users.Add(user);
        }
        await context.SaveChangesAsync();
    }
}
