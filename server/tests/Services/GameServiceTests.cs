using System.Globalization;
using api.Services;
using dataccess;
using dataccess.Repositories;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Xunit;

namespace tests.Services;

public class GameServiceTests
{
    // ----------------------------
    // Helpers: Db + Service factory
    // ----------------------------

    private static MyDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N"))
            .EnableSensitiveDataLogging()
            .Options;

        return new MyDbContext(options);
    }

    private static GameService CreateService(MyDbContext db)
    {
        IRepository<Game> repo = new EfRepository<Game>(db);
        ISieveProcessor sieve = new FakeSieveProcessor();
        return new GameService(repo, sieve);
    }

    private static async Task ResetAsync(MyDbContext db)
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    private static dataccess.Entities.User CreateUser(string suffix = "player") => new()
    {
        Id = Guid.NewGuid(),
        Name = "Player",
        Email = $"{suffix}@test.local",
        PasswordHash = "hashed",
        PhoneNumber = "12345678",
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private static Game CreateGameForWeek(DateTime utc, int weekOffset = 0)
    {
        var week = ISOWeek.GetWeekOfYear(utc) + weekOffset;
        var year = utc.Year;

        return new Game
        {
            Id = Guid.NewGuid(),
            Year = year,
            WeekNumber = week,
            StartTime = utc.AddDays(-1),
            BetDeadline = utc.AddDays(1),
            WinningNumbers = null,
            DrawDate = null,

            InPersonWinners = 0,
            InPersonPrizePool = 0,

            Bets = new List<Bet>(),
        };
    }

    // ----------------------------
    // Tests
    // ----------------------------

    [Fact]
    public async Task GetGameById_Happy_ReturnsDtoWithBetsUserAndTransaction()
    {
        await using var db = CreateDb();
        await ResetAsync(db);

        var service = CreateService(db);

        var now = DateTime.UtcNow;
        var user = CreateUser();
        var game = CreateGameForWeek(now);

        var tx = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Amount = 50,
            Type = TransactionType.Deposit, 
            Status = TransactionStatus.Pending
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
            SelectedNumbers = "1,2,3,4,5",
            CreatedAt = DateTime.UtcNow
        };

        db.Users.Add(user);
        db.Games.Add(game);
        db.Transactions.Add(tx);
        db.Bets.Add(bet);
        await db.SaveChangesAsync();

        var dto = await service.GetGameById(game.Id);

        Assert.NotNull(dto);
        Assert.Single(dto!.Bets);
        Assert.NotNull(dto.Bets.First().User);
        Assert.NotNull(dto.Bets.First().Transaction);
    }

    [Fact]
    public async Task GetGameById_Unhappy_ReturnsNull()
    {
        await using var db = CreateDb();
        await ResetAsync(db);

        var service = CreateService(db);

        var dto = await service.GetGameById(Guid.NewGuid());
        Assert.Null(dto);
    }

    [Fact]
    public async Task GetCurrentGame_Unhappy_ThrowsWhenMissing()
    {
        await using var db = CreateDb();
        await ResetAsync(db);

        var service = CreateService(db);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetCurrentGame());
    }

    [Fact]
    public async Task GetAllUpcomingGames_Happy_ReturnsOnlyUpcoming()
    {
        await using var db = CreateDb();
        await ResetAsync(db);

        var service = CreateService(db);

        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentWeek = ISOWeek.GetWeekOfYear(now);

        var pastGame = new Game
        {
            Id = Guid.NewGuid(),
            Year = currentYear,
            WeekNumber = currentWeek - 1,
            StartTime = now.AddDays(-10),
            BetDeadline = now.AddDays(-9),
            WinningNumbers = "1,2,3,4,5",
            DrawDate = now.AddDays(-8),

            InPersonWinners = 0,
            InPersonPrizePool = 0,
            Bets = new List<Bet>(),
        };

        var upcomingGame = new Game
        {
            Id = Guid.NewGuid(),
            Year = currentYear,
            WeekNumber = currentWeek,
            StartTime = now.AddDays(-1),
            BetDeadline = now.AddDays(1),
            WinningNumbers = null,
            DrawDate = null,

            InPersonWinners = 0,
            InPersonPrizePool = 0,
            Bets = new List<Bet>(),
        };

        db.Games.AddRange(pastGame, upcomingGame);
        await db.SaveChangesAsync();

        var result = await service.GetAllUpcomingGames(new SieveModel { Page = 1, PageSize = 50 });

        Assert.Single(result.Items);
        Assert.Equal(upcomingGame.Id, result.Items[0].Id);
    }

    [Fact]
    public async Task NotImplementedMethods_Throw()
    {
        await using var db = CreateDb();
        await ResetAsync(db);

        var service = CreateService(db);

        await Assert.ThrowsAsync<NotImplementedException>(() => service.GetDigitalWinningBetsAsync(Guid.NewGuid()));
        await Assert.ThrowsAsync<NotImplementedException>(() => service.GetAllWinningBetsAsync(Guid.NewGuid()));
        await Assert.ThrowsAsync<NotImplementedException>(() => service.SeedFutureGamesIfNeededAsync());
    }

    // ----------------------------
    // Minimal test implementations
    // ----------------------------

    private sealed class EfRepository<T> : IRepository<T> where T : class
    {
        private readonly MyDbContext _db;
        public EfRepository(MyDbContext db) => _db = db;

        public Task<T?> GetAsync(Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Query() => _db.Set<T>();

        public async Task AddAsync(T entity)
        {
            _db.Set<T>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Set<T>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }

    private sealed class FakeSieveProcessor : ISieveProcessor
    {
        public IQueryable<TEntity> Apply<TEntity>(
            SieveModel model,
            IQueryable<TEntity> source,
            object[]? dataForCustomMethods = null,
            bool applyFiltering = true,
            bool applySorting = true,
            bool applyPagination = true)
        {
            if (!applyPagination) return source;

            var page = model.Page ?? 1;
            var pageSize = model.PageSize ?? 10;

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
