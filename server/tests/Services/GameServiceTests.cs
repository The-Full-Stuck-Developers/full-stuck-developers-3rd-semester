using System.Globalization;
using api.Services;
using dataccess;
using dataccess.Entities;
using dataccess.Repositories;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sieve.Services;

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
    
    private static GameService CreateEfBackedService(MyDbContext db)
    {
        var repo = new Mock<IRepository<Game>>();

        repo.Setup(r => r.Query()).Returns(db.Set<Game>());

        repo.Setup(r => r.UpdateAsync(It.IsAny<Game>()))
            .Returns<Game>(async g =>
            {
                db.Set<Game>().Update(g);
                await db.SaveChangesAsync();
            });

        repo.Setup(r => r.AddAsync(It.IsAny<Game>()))
            .Returns<Game>(async g =>
            {
                await db.Set<Game>().AddAsync(g);
                await db.SaveChangesAsync();
            });

        return new GameService(repo.Object, Mock.Of<ISieveProcessor>());
    }

    
    // -------------------------
    // Seed helpers
    // -------------------------

    private static User CreateValidUser(string suffix = "player")
        => new()
        {
            Id = Guid.NewGuid(),
            Name = "Player",
            Email = $"{suffix}@test.local",
            PasswordHash = "hashed",
            PhoneNumber = "12345678",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

    private static Game CreateCurrentWeekGame()
    {
        var now = DateTime.UtcNow;
        return new Game
        {
            Id = Guid.NewGuid(),
            Year = now.Year,
            WeekNumber = ISOWeek.GetWeekOfYear(now),
            StartTime = now.AddDays(-1),
            BetDeadline = now.AddHours(1),
            WinningNumbers = null,
            NumberOfPhysicalPlayers = 0
        };
    }

    private static Bet CreateValidBet(Game game, User user)
    {
        var tx = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Amount = 50,
        };

        var bet = new Bet
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            Game = game,
            UserId = user.Id,
            User = user,
            TransactionId = tx.Id,
            Transaction = tx,
            SelectedNumbers = "1,2,3,4,5"
        };

        tx.Bet = bet;

        game.Bets.Add(bet);
        user.Bets.Add(bet);

        return bet;
    }

    // ----------------------------
    // GetGameById (happy + unhappy)
    // ----------------------------

    [Fact]
    public async Task GetGameById_GameExists_ReturnsDtoWithBetsAndUser()
    {
        using var db = CreateDbContext();

        var user = CreateValidUser();
        var game = CreateCurrentWeekGame();
        var bet = CreateValidBet(game, user);

        db.Users.Add(user);
        db.Games.Add(game);
        db.Set<Transaction>().Add(bet.Transaction);
        db.Set<Bet>().Add(bet);
        await db.SaveChangesAsync();

        var service = CreateEfBackedService(db);

        var result = await service.GetGameById(game.Id);

        Assert.NotNull(result);
        Assert.Single(result!.Bets);
        Assert.NotNull(result.Bets.First().User);
    }

    [Fact]
    public async Task GetGameById_GameNotFound_ReturnsNull()
    {
        using var db = CreateDbContext();
        var service = CreateEfBackedService(db);

        var result = await service.GetGameById(Guid.NewGuid());

        Assert.Null(result);
    }

    // --------------------------------
    // GetCurrentGame (happy + unhappy)
    // --------------------------------

    [Fact]
    public async Task GetCurrentGame_GameExists_ReturnsGameWithBetsAndUser()
    {
        using var db = CreateDbContext();

        var user = CreateValidUser();
        var game = CreateCurrentWeekGame();
        var bet = CreateValidBet(game, user);

        db.Users.Add(user);
        db.Games.Add(game);
        db.Set<Transaction>().Add(bet.Transaction);
        db.Set<Bet>().Add(bet);
        await db.SaveChangesAsync();

        var service = CreateEfBackedService(db);

        var result = await service.GetCurrentGame();

        Assert.NotNull(result);
        Assert.Single(result.Bets);
        Assert.NotNull(result.Bets.First().User);
    }

    [Fact]
    public async Task GetCurrentGame_GameNotFound_ThrowsKeyNotFoundException()
    {
        using var db = CreateDbContext();
        var service = CreateEfBackedService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetCurrentGame());
    }

    // ------------------------------------------------
    // GetOrCreateCurrentGameAsync (happy + unhappy)
    // ------------------------------------------------

    [Fact]
    public async Task GetOrCreateCurrentGameAsync_CanBetGameExists_ReturnsIt()
    {
        using var db = CreateDbContext();
        var now = DateTime.UtcNow;

        var canBetGame = new Game
        {
            Id = Guid.NewGuid(),
            Year = now.Year,
            WeekNumber = ISOWeek.GetWeekOfYear(now),
            StartTime = now.AddMinutes(10),
            BetDeadline = now.AddHours(2),
            WinningNumbers = null,
            NumberOfPhysicalPlayers = 0
        };

        db.Games.Add(canBetGame);
        await db.SaveChangesAsync();

        var service = CreateEfBackedService(db);

        var result = await service.GetOrCreateCurrentGameAsync();

        Assert.Equal(canBetGame.Id, result.Id);
    }

    [Fact]
    public async Task GetOrCreateCurrentGameAsync_NoOpenGames_CreatesNewGame()
    {
        using var db = CreateDbContext();
        var service = CreateEfBackedService(db);

        var game = await service.GetOrCreateCurrentGameAsync();

        Assert.NotNull(game);
        Assert.True(game.BetDeadline > DateTime.UtcNow);
    }
}
