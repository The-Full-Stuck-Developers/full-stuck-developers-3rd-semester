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

        var now = DateTime.UtcNow;

        var games = new List<Game>
        {
            new Game
            {
                Id = Guid.NewGuid(),
                StartTime = now.AddDays(-1),
                BetDeadline = now.AddDays(-2),
                DrawDate = now.AddDays(-1),
                WeekNumber = 42,
                Year = now.Year,
                WinningNumbers = "1,5,10,14,22",
            },
            new Game
            {
                Id = Guid.NewGuid(),
                StartTime = now.AddMinutes(-30),
                BetDeadline = now.AddHours(-1),
                DrawDate = now,
                WeekNumber = 43,
                Year = now.Year,
                WinningNumbers = "4,1,16,8,23",
            }
        };

        await db.Games.AddRangeAsync(games);
        await db.SaveChangesAsync();
    }
}
