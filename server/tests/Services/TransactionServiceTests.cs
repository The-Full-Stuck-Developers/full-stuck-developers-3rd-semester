using api.Services;
using api.Models.Dtos.Requests.Transaction;
using dataccess;
using dataccess.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Xunit;

public class TransactionServiceTests : IDisposable
{
    private readonly MyDbContext _db;
    private readonly ISieveProcessor _sieveProcessor;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new MyDbContext(options);

        _sieveProcessor = new SieveProcessor(new SieveOptionsAccessor());

        _service = new TransactionService(_db, _sieveProcessor);
    }

    public void Dispose()
    {
        _db.Dispose();
    }

    private async Task ResetAsync()
    {
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();
    }

    private static SieveModel DefaultSieve() => new()
    {
        Page = 1,
        PageSize = 50
    };

    private static User CreateValidUser(string suffix = "u") => new()
    {
        Id = Guid.NewGuid(),
        Name = "User",
        Email = $"{suffix}@test.local",
        PasswordHash = "hashed",
        PhoneNumber = "12345678",
        IsActive = true,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private static Transaction CreateValidTransaction(User user, TransactionType type, TransactionStatus status, int amount = 100)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            User = user,
            Type = type,
            Status = status,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        };

    [Fact]
    public async Task GetAllTransactions_ReturnsOnlyDeposits()
    {
        await ResetAsync();

        var u1 = CreateValidUser("u1");
        var u2 = CreateValidUser("u2");
        _db.Users.AddRange(u1, u2);

        _db.Transactions.AddRange(
            CreateValidTransaction(u1, TransactionType.Deposit, TransactionStatus.Accepted, 100),
            CreateValidTransaction(u2, TransactionType.Purchase, TransactionStatus.Accepted, 50)
        );

        await _db.SaveChangesAsync();

        var result = await _service.GetAllTransactions(DefaultSieve());

        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
        Assert.All(result.Items, t => Assert.Equal(TransactionType.Deposit, t.Type));
    }

    [Fact]
    public async Task GetTransactionsByUser_ReturnsOnlyUserDeposits()
    {
        await ResetAsync();

        var user = CreateValidUser("target");
        var other = CreateValidUser("other");
        _db.Users.AddRange(user, other);

        _db.Transactions.AddRange(
            CreateValidTransaction(user, TransactionType.Deposit, TransactionStatus.Accepted, 100),
            CreateValidTransaction(other, TransactionType.Deposit, TransactionStatus.Accepted, 200)
        );

        await _db.SaveChangesAsync();

        var result = await _service.GetTransactionsByUser(user.Id, DefaultSieve());

        Assert.Single(result.Items);
        Assert.Equal(user.Id, result.Items.First().UserId);
        Assert.All(result.Items, t => Assert.Equal(TransactionType.Deposit, t.Type));
    }

    [Fact]
    public async Task CreateTransaction_ValidDto_CreatesPendingTransaction()
    {
        await ResetAsync();

        var userId = Guid.NewGuid();
        _db.Users.Add(new User
        {
            Id = userId,
            Name = "User",
            Email = "user@test.local",
            PasswordHash = "hashed",
            PhoneNumber = "12345678",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        var dto = new CreateTransactionDto
        {
            UserId = userId.ToString(),
            Amount = 500,
            MobilePayTransactionNumber = 552255998
        };

        var result = await _service.CreateTransaction(dto);

        Assert.Equal(TransactionStatus.Pending, result.Status);
        Assert.Equal(1, _db.Transactions.Count());
    }

    [Fact]
    public async Task UpdateTransactionStatus_UpdatesStatus()
    {
        await ResetAsync();

        var user = CreateValidUser("u");
        _db.Users.Add(user);

        var transaction = CreateValidTransaction(user, TransactionType.Deposit, TransactionStatus.Pending, 100);
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        var result = await _service.UpdateTransactionStatus(
            transaction.Id,
            new UpdateTransactionDto { Status = TransactionStatus.Accepted });

        Assert.NotNull(result);
        Assert.Equal(TransactionStatus.Accepted, result!.Status);
    }

    [Fact]
    public async Task DeleteTransaction_SetsStatusCancelled()
    {
        await ResetAsync();

        var user = CreateValidUser("u");
        _db.Users.Add(user);

        var transaction = CreateValidTransaction(user, TransactionType.Deposit, TransactionStatus.Pending, 100);
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        await _service.DeleteTransaction(transaction.Id);

        Assert.Equal(TransactionStatus.Cancelled, transaction.Status);
    }

    [Fact]
    public async Task ApproveTransaction_SetsAccepted()
    {
        await ResetAsync();

        var user = CreateValidUser("u");
        _db.Users.Add(user);

        var transaction = CreateValidTransaction(user, TransactionType.Deposit, TransactionStatus.Pending, 100);
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        var result = await _service.ApproveTransaction(transaction.Id);

        Assert.Equal(TransactionStatus.Accepted, result.Status);
    }

    [Fact]
    public async Task RejectTransaction_SetsRejected()
    {
        await ResetAsync();

        var user = CreateValidUser("u");
        _db.Users.Add(user);

        var transaction = CreateValidTransaction(user, TransactionType.Deposit, TransactionStatus.Pending, 100);
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        var result = await _service.RejectTransaction(transaction.Id);

        Assert.Equal(TransactionStatus.Rejected, result.Status);
    }

    [Fact]
    public async Task GetPendingTransactionsCount_ReturnsCorrectCount()
    {
        await ResetAsync();

        var user = CreateValidUser("u");
        _db.Users.Add(user);

        _db.Transactions.AddRange(
            CreateValidTransaction(user, TransactionType.Deposit, TransactionStatus.Pending, 100),
            CreateValidTransaction(user, TransactionType.Deposit, TransactionStatus.Accepted, 100)
        );
        await _db.SaveChangesAsync();

        var count = await _service.GetPendingTransactionsCount();

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetUserBalance_CalculatesCorrectly()
    {
        await ResetAsync();

        var user = CreateValidUser("u");
        _db.Users.Add(user);

        _db.Transactions.AddRange(
            CreateValidTransaction(user, TransactionType.Deposit, TransactionStatus.Accepted, 100),
            CreateValidTransaction(user, TransactionType.Purchase, TransactionStatus.Accepted, 40)
        );
        await _db.SaveChangesAsync();

        var balance = await _service.GetUserBalance(user.Id);

        Assert.Equal(60, balance);
    }
}

public class SieveOptionsAccessor : Microsoft.Extensions.Options.IOptions<SieveOptions>
{
    public SieveOptions Value => new()
    {
        DefaultPageSize = 50,
        MaxPageSize = 200
    };
}
