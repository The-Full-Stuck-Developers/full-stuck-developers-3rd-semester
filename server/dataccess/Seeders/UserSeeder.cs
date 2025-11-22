using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dataccess;
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
                Id = Guid.NewGuid().ToString(),
                Name = "Admin User",
                Email = "admin@example.com",
                Password = "hashed_password_here",
                PhoneNumber = "12345678",
                IsAdmin = true,
                CreatedAt = DateTime.UtcNow,
                DeletedAt = null,
                ExpiresAt = null,
            },
            new User
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Regular User",
                Email = "user@example.com",
                Password = "hashed_password_here",
                PhoneNumber = "87654321",
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow,
                DeletedAt = null,
                ExpiresAt = null,
            }
        };

        await db.Users.AddRangeAsync(users);
        await db.SaveChangesAsync();
    }
}