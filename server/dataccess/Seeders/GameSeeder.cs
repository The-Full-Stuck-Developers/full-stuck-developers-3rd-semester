using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dataccess;
using dataccess.Seeders;

namespace dataccess.Seeders;

public class GameSeeder : ISeeder
{
    public async Task SeedAsync(MyDbContext db)
    {
        if (db.Games.Any()) return;

        var games = new List<Game>
        {
            new Game
            {
                Id = Guid.NewGuid().ToString(),
                StartTime = DateTime.UtcNow.AddDays(-1),
                IsActive = false,
                WinningNumbers = "1,5,10,14,22",
                Revenue = 10000
            },
            new Game
            {
                Id = Guid.NewGuid().ToString(),
                StartTime = DateTime.UtcNow.AddMinutes(-30),
                IsActive = true,
                WinningNumbers = "4,1,16,8,23",
                Revenue = 5500
            }
        };

        await db.Games.AddRangeAsync(games);
        await db.SaveChangesAsync();
    }
}