using api.Models.Dtos.Requests.Game;
using api.Services;
using dataccess;
using dataccess.Entities;
using dataccess.Repositories;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Xunit;
using System.Globalization;

namespace api.Tests.Services;

public class GameServiceTests
{
    private readonly MyDbContext _db;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly IRepository<Game> _gameRepository;
    private readonly GameService _service;

    public GameServiceTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new MyDbContext(options);
        _sieveProcessor = new SieveProcessor(new SieveOptionsAccessor());
        _gameRepository = new GameRepository(_db);
        _service = new GameService(_gameRepository, _sieveProcessor);
    }

    [Fact]
    public async Task GetCurrentGame_GameExists_ReturnsGameForCurrentWeek()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentWeek = ISOWeek.GetWeekOfYear(now);

        var user = new User 
        { 
            Id = Guid.NewGuid(), 
            Name = "Player",
            Email = "player@test.com",
            PasswordHash = "hash",
            PhoneNumber = "123",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Year = currentYear,
            WeekNumber = currentWeek,
            StartTime = now,
            BetDeadline = now.AddDays(1)
        };

        _db.Users.Add(user);
        _db.Games.Add(game);
        await _db.SaveChangesAsync();

        // Act
        var result = await _service.GetCurrentGame();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(currentYear, result.Year);
        Assert.Equal(currentWeek, result.WeekNumber);
    }
    
    [Fact]
    public async Task GetCurrentGame_GameNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange - no game created for current week

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.GetCurrentGame());
    }

    [Fact]
    public async Task GetOrCreateCurrentGameAsync_FutureGameExists_ReturnsIt()
    {
        // Arrange
        var now = DateTime.UtcNow;

        var futureGame = new Game
        {
            Id = Guid.NewGuid(),
            Year = now.Year,
            WeekNumber = ISOWeek.GetWeekOfYear(now) + 1,
            WinningNumbers = null,
            BetDeadline = now.AddDays(1),
            StartTime = now.AddHours(2)
        };

        _db.Games.Add(futureGame);
        await _db.SaveChangesAsync();

        // Act
        var result = await _service.GetOrCreateCurrentGameAsync();

        // Assert
        Assert.Equal(futureGame.Id, result.Id);
    }

    [Fact]
    public async Task GetOrCreateCurrentGameAsync_NoFutureGames_ThrowsInvalidOperationException()
    {
        // Arrange - no games created

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.GetOrCreateCurrentGameAsync());
    }

    [Fact]
    public async Task GetAllUpcomingGames_ReturnsOnlyFutureGames()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentWeek = ISOWeek.GetWeekOfYear(now);

        _db.Games.AddRange(
            new Game
            {
                Id = Guid.NewGuid(),
                Year = currentYear,
                WeekNumber = currentWeek + 1,
                StartTime = now.AddDays(7),
                BetDeadline = now.AddDays(7)
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Year = currentYear,
                WeekNumber = currentWeek - 1,
                StartTime = now.AddDays(-7),
                BetDeadline = now.AddDays(-7)
            }
        );

        await _db.SaveChangesAsync();

        // Act
        var result = await _service.GetAllUpcomingGames(new SieveModel());

        // Assert
        Assert.Single(result.Items);
        Assert.True(result.Items.First().WeekNumber >= currentWeek);
    }

    [Fact]
    public async Task GetAllPastGames_ReturnsOnlyPastGames()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentWeek = ISOWeek.GetWeekOfYear(now);

        _db.Games.AddRange(
            new Game
            {
                Id = Guid.NewGuid(),
                Year = currentYear,
                WeekNumber = currentWeek - 1,
                StartTime = now.AddDays(-7),
                BetDeadline = now.AddDays(-7)
            },
            new Game
            {
                Id = Guid.NewGuid(),
                Year = currentYear,
                WeekNumber = currentWeek + 1,
                StartTime = now.AddDays(7),
                BetDeadline = now.AddDays(7)
            }
        );

        await _db.SaveChangesAsync();

        // Act
        var result = await _service.GetAllPastGames(new SieveModel());

        // Assert
        Assert.Single(result.Items);
        Assert.True(result.Items.First().WeekNumber < currentWeek);
    }

    [Fact]
    public async Task UpdateWinningNumbers_ValidGame_UpdatesNumbers()
    {
        // Arrange
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Year = 2024,
            WeekNumber = 1,
            StartTime = DateTime.UtcNow,
            BetDeadline = DateTime.UtcNow
        };

        _db.Games.Add(game);
        await _db.SaveChangesAsync();

        var dto = new WinningNumbersDto { WinningNumbers = "1,2,3,4,5,6,7" };

        // Act
        var result = await _service.UpdateWinningNumbers(game.Id, dto);

        // Assert
        Assert.NotNull(result.WinningNumbers);
        Assert.Contains("1", result.WinningNumbers);
    }

    [Fact]
    public async Task DrawWinners_GameWithoutWinningNumbers_ThrowsInvalidOperationException()
    {
        // Arrange
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Year = 2024,
            WeekNumber = 1,
            StartTime = DateTime.UtcNow,
            BetDeadline = DateTime.UtcNow,
            WinningNumbers = null
        };

        _db.Games.Add(game);
        await _db.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.DrawWinners(game.Id));
    }
}

// Simple repository implementation for testing
public class GameRepository : IRepository<Game>
{
    private readonly MyDbContext _context;

    public GameRepository(MyDbContext context)
    {
        _context = context;
    }

    public Task<Game?> GetAsync(Func<Game, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public IQueryable<Game> Query()
    {
        return _context.Games.AsQueryable();
    }

    public async Task AddAsync(Game entity)
    {
        await _context.Games.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game entity)
    {
        _context.Games.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Game entity)
    {
        _context.Games.Remove(entity);
        await _context.SaveChangesAsync();
    }
}

// Helper class to provide Sieve options
public class SieveOptionsAccessor : Microsoft.Extensions.Options.IOptions<SieveOptions>
{
    public SieveOptions Value => new SieveOptions();
}