using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Bogus;
using dataccess;
using dataccess.Entities;
using dataccess.Seeders;

namespace dataccess.Seeders;

public class UserSeeder : ISeeder
{
    public async Task SeedAsync(MyDbContext db)
    {
        if (db.Users.Any()) return;

        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Name = "Admin User",
                Email = "admin@example.com",
                PasswordHash = "hashed_password_here",
                PhoneNumber = "12345678",
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow,
                DeletedAt = null,
                ExpiresAt = null,
            },
            new User
            {
                Id = Guid.NewGuid(),
                Name = "Regular User",
                Email = "user@example.com",
                PasswordHash = "hashed_password_here",
                PhoneNumber = "87654321",
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow,
                DeletedAt = null,
                ExpiresAt = DateTime.UtcNow.AddMonths(new Random().Next(1, 12)),
            }
        };

        var faker = new Faker<User>()
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.Name, f => f.Name.FullName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PasswordHash, f => "hashed_password_here")
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber("########"))
            .RuleFor(u => u.IsAdmin, f => false)
            .RuleFor(u => u.CreatedAt, f => f.Date.Past(1).ToUniversalTime())
            .RuleFor(u => u.DeletedAt, f => (DateTime?)null)
            .RuleFor(u => u.ExpiresAt, f => f.Date.Future(1).ToUniversalTime());

        var randomUsers = faker.Generate(50);
        users.AddRange(randomUsers);

        await db.Users.AddRangeAsync(users);
        await db.SaveChangesAsync();
    }
}