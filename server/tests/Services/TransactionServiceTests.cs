using api.Services;
using api.Models;
using api.Models.Dtos.Requests.Transaction;
using dataccess;
using dataccess.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;
using Xunit;

public class TransactionServiceTests
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

        // IMPORTANT: give Sieve a non-zero DefaultPageSize, otherwise Apply() can return 0 items
        _sieveProcessor = new SieveProcessor(new SieveOptionsAccessor());

        _service = new TransactionService(_db, _sieveProcessor);
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
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task GetAllTransactions_ReturnsOnlyDeposits()
    {
        // Arrange
        var u1 = CreateValidUser("u1");
        var u2 = CreateValidUser("u2");
        _db.Users.AddRange(u1, u2);

        _db.Transactions.AddRange(
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = u1.Id,
                User = u1,
                Type = TransactionType.Deposit,
                Amount = 100,
                Status = TransactionStatus.Accepted,
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = u2.Id,
                User = u2,
                Type = TransactionType.Purchase,
                Amount = 50,
                Status = TransactionStatus.Accepted,
                CreatedAt = DateTime.UtcNow
            }
        );

        await _db.SaveChangesAsync();

        // Act
        var result = await _service.GetAllTransactions(DefaultSieve());

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
        Assert.All(result.Items, t => Assert.Equal(TransactionType.Deposit, t.Type));
    }

    [Fact]
    public async Task GetTransactionsByUser_ReturnsOnlyUserDeposits()
    {
        // Arrange
        var user = CreateValidUser("target");
        var other = CreateValidUser("other");
        _db.Users.AddRange(user, other);

        _db.Transactions.AddRange(
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                User = user,
                Type = TransactionType.Deposit,
                Amount = 100,
                Status = TransactionStatus.Accepted,
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = other.Id,
                User = other,
                Type = TransactionType.Deposit,
                Amount = 200,
                Status = TransactionStatus.Accepted,
                CreatedAt = DateTime.UtcNow
            }
        );

        await _db.SaveChangesAsync();

        // Act
        var result = await _service.GetTransactionsByUser(user.Id, DefaultSieve());

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(user.Id, result.Items.First().UserId);
        Assert.All(result.Items, t => Assert.Equal(TransactionType.Deposit, t.Type));
    }

    [Fact]
    public async Task CreateTransaction_ValidDto_CreatesPendingTransaction()
    {
        var dto = new CreateTransactionDto
        {
            UserId = Guid.NewGuid().ToString(),
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
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Status = TransactionStatus.Pending
        };
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
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Status = TransactionStatus.Pending
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        await _service.DeleteTransaction(transaction.Id);

        Assert.Equal(TransactionStatus.Cancelled, transaction.Status);
    }

    [Fact]
    public async Task ApproveTransaction_SetsAccepted()
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Status = TransactionStatus.Pending
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        var result = await _service.ApproveTransaction(transaction.Id);

        Assert.Equal(TransactionStatus.Accepted, result.Status);
    }

    [Fact]
    public async Task RejectTransaction_SetsRejected()
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Status = TransactionStatus.Pending
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        var result = await _service.RejectTransaction(transaction.Id);

        Assert.Equal(TransactionStatus.Rejected, result.Status);
    }

    [Fact]
    public async Task GetPendingTransactionsCount_ReturnsCorrectCount()
    {
        _db.Transactions.AddRange(
            new Transaction { Status = TransactionStatus.Pending },
            new Transaction { Status = TransactionStatus.Accepted }
        );
        await _db.SaveChangesAsync();

        var count = await _service.GetPendingTransactionsCount();

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetUserBalance_CalculatesCorrectly()
    {
        var userId = Guid.NewGuid();
        _db.Transactions.AddRange(
            new Transaction
            {
                UserId = userId,
                Type = TransactionType.Deposit,
                Status = TransactionStatus.Accepted,
                Amount = 100
            },
            new Transaction
            {
                UserId = userId,
                Type = TransactionType.Purchase,
                Amount = 40
            }
        );
        await _db.SaveChangesAsync();

        var balance = await _service.GetUserBalance(userId);

        Assert.Equal(60, balance);
    }
}

// Helper class to provide Sieve options
public class SieveOptionsAccessor : Microsoft.Extensions.Options.IOptions<SieveOptions>
{
    public SieveOptions Value => new()
    {
        DefaultPageSize = 50,
        MaxPageSize = 200
    };
}
