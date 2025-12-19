using api.Services;
using dataccess;
using dataccess.Entities;
using DefaultNamespace;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace tests.Services;

public class BoardServiceTests
{
    private readonly MyDbContext _db;
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly BoardService _service;

    public BoardServiceTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new MyDbContext(options);
        _gameServiceMock = new Mock<IGameService>();
        _service = new BoardService(_db, _gameServiceMock.Object);
    }

    private async Task ResetAsync()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();
    }

    private static User CreateUser(string suffix = "u") => new()
    {
        Id = Guid.NewGuid(),
        Name = "User",
        Email = $"{suffix}@test.local",
        PasswordHash = "hashed",
        PhoneNumber = "12345678",
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };

    private static Game CreateGame(DateTime now) => new()
    {
        Id = Guid.NewGuid(),
        Year = now.Year,
        WeekNumber = System.Globalization.ISOWeek.GetWeekOfYear(now),
        StartTime = now.AddDays(-1),
        BetDeadline = now.AddDays(1),
        WinningNumbers = null,
        DrawDate = null,
        InPersonWinners = 0,
        InPersonPrizePool = 0,
        Bets = new List<Bet>()
    };

    private static CreateBetDto NewCreateBetDto(List<int> numbers, int price, int count = 5, int repeatWeeks = 1)
        => new(numbers, count, price, repeatWeeks);

    [Fact]
    public async Task GetAllBoards_ReturnsBoardsFromAllBets_InDescendingCreatedAt()
    {
        await ResetAsync();

        var user1 = CreateUser("u1");
        var user2 = CreateUser("u2");
        _db.Users.AddRange(user1, user2);

        var betOld = new Bet
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            User = user1,
            GameId = Guid.NewGuid(),
            Game = null!,
            TransactionId = Guid.NewGuid(),
            Transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = user1.Id,
                User = user1,
                Amount = 25,
                Type = TransactionType.Purchase,
                Status = TransactionStatus.Accepted,
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            },
            SelectedNumbers = "5,2,9,1,7",
            CreatedAt = DateTime.UtcNow.AddHours(-2)
        };

        var betNew = new Bet
        {
            Id = Guid.NewGuid(),
            UserId = user2.Id,
            User = user2,
            GameId = Guid.NewGuid(),
            Game = null!,
            TransactionId = Guid.NewGuid(),
            Transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = user2.Id,
                User = user2,
                Amount = 25,
                Type = TransactionType.Purchase,
                Status = TransactionStatus.Accepted,
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            },
            SelectedNumbers = "9,1,5,3,7",
            CreatedAt = DateTime.UtcNow.AddHours(-1)
        };

        _db.Transactions.AddRange(betOld.Transaction, betNew.Transaction);
        _db.Bets.AddRange(betOld, betNew);
        await _db.SaveChangesAsync();

        var result = await _service.GetAllBoards();

        Assert.NotNull(result);
        Assert.Equal(2, result.Total);
        Assert.Equal(2, result.Items.Count);

        Assert.Equal(new List<int> { 9, 1, 5, 3, 7 }, result.Items[0].Numbers);
        Assert.Equal(new List<int> { 5, 2, 9, 1, 7 }, result.Items[1].Numbers);
    }

    [Fact]
    public async Task GetBoardsByUser_ReturnsOnlyThatUsersBoards()
    {
        await ResetAsync();

        var user1 = CreateUser("u1");
        var user2 = CreateUser("u2");
        _db.Users.AddRange(user1, user2);

        var bet1 = new Bet
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id,
            User = user1,
            GameId = Guid.NewGuid(),
            Game = null!,
            TransactionId = Guid.NewGuid(),
            Transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = user1.Id,
                User = user1,
                Amount = 25,
                Type = TransactionType.Purchase,
                Status = TransactionStatus.Accepted
            },
            SelectedNumbers = "1,2,3,4,5",
            CreatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        var bet2 = new Bet
        {
            Id = Guid.NewGuid(),
            UserId = user2.Id,
            User = user2,
            GameId = Guid.NewGuid(),
            Game = null!,
            TransactionId = Guid.NewGuid(),
            Transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = user2.Id,
                User = user2,
                Amount = 25,
                Type = TransactionType.Purchase,
                Status = TransactionStatus.Accepted
            },
            SelectedNumbers = "6,7,8,9,10",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5)
        };

        _db.Transactions.AddRange(bet1.Transaction, bet2.Transaction);
        _db.Bets.AddRange(bet1, bet2);
        await _db.SaveChangesAsync();

        var result = await _service.GetBoardsByUser(user1.Id);

        Assert.NotNull(result);
        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        Assert.Equal(new List<int> { 1, 2, 3, 4, 5 }, result.Items[0].Numbers);
    }

    [Fact]
    public async Task CreateBet_CreatesBet_ForCurrentGame_AndReturnsBetId()
    {
        await ResetAsync();

        var now = DateTime.UtcNow;
        var user = CreateUser("u1");
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var currentGame = CreateGame(now);

        _gameServiceMock
            .Setup(g => g.GetOrCreateCurrentGameAsync())
            .ReturnsAsync(currentGame);

        var tx = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Amount = 25,
            Type = TransactionType.Purchase,
            Status = TransactionStatus.Accepted
        };
        _db.Transactions.Add(tx);
        await _db.SaveChangesAsync();

        var dto = NewCreateBetDto(new List<int> { 5, 2, 9, 1, 7 }, price: 25, count: 5);

        var betId = await _service.CreateBet(dto, user.Id, tx);

        var bet = await _db.Bets.FirstOrDefaultAsync(b => b.Id == betId);
        Assert.NotNull(bet);
        Assert.Equal(user.Id, bet!.UserId);
        Assert.Equal(currentGame.Id, bet.GameId);
        Assert.Equal(tx.Id, bet.TransactionId);

        Assert.Equal("1,2,5,7,9", bet.SelectedNumbers);
    }

    [Fact]
    public async Task CreateBetsForWeeks_CreatesOneBetPerWeek_WithSeriesId_AndTransactions()
    {
        await ResetAsync();

        var now = DateTime.UtcNow;
        var user = CreateUser("u1");
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var games = new List<Game>
        {
            CreateGame(now),
            CreateGame(now.AddDays(7)),
            CreateGame(now.AddDays(14))
        };

        _gameServiceMock
            .Setup(g => g.GetOrCreateGamesForWeeksAsync(3))
            .ReturnsAsync(games);

        var dto = NewCreateBetDto(new List<int> { 3, 1, 5, 2, 4 }, price: 25, count: 5, repeatWeeks: 3);

        var betIds = await _service.CreateBetsForWeeks(dto, user.Id, repeatWeeks: 3);

        Assert.Equal(3, betIds.Count);

        var bets = await _db.Bets
            .Where(b => betIds.Contains(b.Id))
            .ToListAsync();

        Assert.Equal(3, bets.Count);

        Assert.All(bets, b =>
        {
            Assert.Equal(user.Id, b.UserId);
            Assert.NotEqual(Guid.Empty, b.TransactionId);
            Assert.Equal("1,2,3,4,5", b.SelectedNumbers);
            Assert.NotNull(b.BetSeriesId);
        });

        var seriesId = bets.First().BetSeriesId;
        Assert.All(bets, b => Assert.Equal(seriesId, b.BetSeriesId));

        var transactions = await _db.Transactions
            .Where(t => bets.Select(b => b.TransactionId).Contains(t.Id))
            .ToListAsync();

        Assert.Equal(3, transactions.Count);
        Assert.All(transactions, t =>
        {
            Assert.Equal(user.Id, t.UserId);
            Assert.Equal(TransactionType.Purchase, t.Type);
            Assert.Equal(TransactionStatus.Accepted, t.Status);
            Assert.Equal(25, t.Amount);
        });
    }

    [Fact]
    public async Task CreateBetsForWeeks_WhenGameServiceReturnsWrongCount_Throws()
    {
        await ResetAsync();

        var user = CreateUser("u1");
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        _gameServiceMock
            .Setup(g => g.GetOrCreateGamesForWeeksAsync(3))
            .ReturnsAsync(new List<Game> { CreateGame(DateTime.UtcNow) }); // wrong count

        var dto = NewCreateBetDto(new List<int> { 1, 2, 3, 4, 5 }, price: 25, count: 5, repeatWeeks: 3);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.CreateBetsForWeeks(dto, user.Id, repeatWeeks: 3));
    }
}
