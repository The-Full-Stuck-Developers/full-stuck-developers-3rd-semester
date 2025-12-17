using api.Services;
using dataccess;
using dataccess.Entities;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

namespace api.Tests.Services;

public class GameServiceTests
{
    private static MyDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new MyDbContext(options);
    }

    [Fact]
    public async Task GetCurrentGameAsync_GameExists_ReturnsGameWithBets()
    {
        using var db = CreateDbContext();

        var user = new User { Id = Guid.NewGuid(), Name = "Player" };
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Bets =
            {
                new Bet
                {
                    Id = Guid.NewGuid(),
                    User = user
                }
            }
        };

        db.Users.Add(user);
        db.Games.Add(game);
        await db.SaveChangesAsync();

        var service = new GameService(db);

        var result = await service.GetCurrentGameAsync(game.Id);

        Assert.NotNull(result);
        Assert.Single(result.Bets);
        Assert.NotNull(result.Bets.First().User);
    }
    
    [Fact]
    public async Task GetCurrentGameAsync_GameNotFound_ThrowsKeyNotFoundException()
    {
        using var db = CreateDbContext();
        var service = new GameService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.GetCurrentGameAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetOrCreateCurrentGameAsync_CanBetGameExists_ReturnsIt()
    {
        using var db = CreateDbContext();

        var game = new Game
        {
            Id = Guid.NewGuid(),
            WinningNumbers = null,
            BetDeadline = DateTime.UtcNow.AddHours(1)
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();

        var service = new GameService(db);

        var result = await service.GetOrCreateCurrentGameAsync();

        Assert.Equal(game.Id, result.Id);
    }

    [Fact]
    public async Task GetOrCreateCurrentGameAsync_NoCanBetGame_ReturnsNextFutureGame()
    {
        using var db = CreateDbContext();
        var now = DateTime.UtcNow;

        var futureGame = new Game
        {
            Id = Guid.NewGuid(),
            WinningNumbers = null,
            BetDeadline = now.AddDays(1),
            StartTime = now.AddHours(2)
        };

        db.Games.Add(futureGame);
        await db.SaveChangesAsync();

        var service = new GameService(db);

        var result = await service.GetOrCreateCurrentGameAsync();

        Assert.Equal(futureGame.Id, result.Id);
    }

    [Fact]
    public async Task GetOrCreateCurrentGameAsync_NoFutureGames_ThrowsInvalidOperationException()
    {
        using var db = CreateDbContext();
        var service = new GameService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.GetOrCreateCurrentGameAsync());
    }

    
}