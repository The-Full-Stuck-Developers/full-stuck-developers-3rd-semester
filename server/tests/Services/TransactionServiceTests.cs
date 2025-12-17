using api.Services;
using api.Models.Dtos.Requests.Transaction;
using dataccess;
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
        
        
        _sieveProcessor = new SieveProcessor(new SieveOptionsAccessor());
        
        _service = new TransactionService(_db, _sieveProcessor);
    }

    [Fact]
    public async Task GetAllTransactions_ReturnsOnlyDeposits()
    {
        // Arrange
        _db.Transactions.AddRange(
            new Transaction
            {
                Id = Guid.NewGuid(),
                Type = TransactionType.Deposit,
                Amount = 100,
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                Id = Guid.NewGuid(),
                Type = TransactionType.Purchase,
                Amount = 50,
                CreatedAt = DateTime.UtcNow
            }
        );
        await _db.SaveChangesAsync();

        // Act
        var result = await _service.GetAllTransactions(new SieveModel());

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(1, result.Total);
    }
    
    [Fact]
    public async Task GetTransactionsByUser_ReturnsOnlyUserDeposits()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _db.Transactions.AddRange(
            new Transaction
            {
                UserId = userId,
                Type = TransactionType.Deposit,
                Amount = 100,
                CreatedAt = DateTime.UtcNow
            },
            new Transaction
            {
                UserId = Guid.NewGuid(),
                Type = TransactionType.Deposit,
                Amount = 200,
                CreatedAt = DateTime.UtcNow
            }
        );
        await _db.SaveChangesAsync();

        // Act
        var result = await _service.GetTransactionsByUser(userId, new SieveModel());

        // Assert
        Assert.Single(result.Items);
        Assert.Equal(userId, result.Items.First().UserId);
    }

    [Fact]
    public async Task CreateTransaction_ValidDto_CreatesPendingTransaction()
    {
        // Arrange
        var dto = new CreateTransactionDto
        {
            UserId = Guid.NewGuid().ToString(),
            Amount = 500,
            MobilePayTransactionNumber = 552255998
        };

        // Act
        var result = await _service.CreateTransaction(dto);

        // Assert
        Assert.Equal(TransactionStatus.Pending, result.Status);
        Assert.Equal(1, _db.Transactions.Count());
    }

    [Fact]
    public async Task UpdateTransactionStatus_UpdatesStatus()
    {
        // Arrange
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Status = TransactionStatus.Pending
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        // Act
        var result = await _service.UpdateTransactionStatus(
            transaction.Id,
            new UpdateTransactionDto { Status = TransactionStatus.Accepted });

        // Assert
        Assert.NotNull(result);
        Assert.Equal(TransactionStatus.Accepted, result.Status);
    }

    [Fact]
    public async Task DeleteTransaction_SetsStatusCancelled()
    {
        // Arrange
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Status = TransactionStatus.Pending
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        // Act
        await _service.DeleteTransaction(transaction.Id);

        // Assert
        Assert.Equal(TransactionStatus.Cancelled, transaction.Status);
    }

    [Fact]
    public async Task ApproveTransaction_SetsAccepted()
    {
        // Arrange
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Status = TransactionStatus.Pending
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        // Act
        var result = await _service.ApproveTransaction(transaction.Id);

        // Assert
        Assert.Equal(TransactionStatus.Accepted, result.Status);
    }

    [Fact]
    public async Task RejectTransaction_SetsRejected()
    {
        // Arrange
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Status = TransactionStatus.Pending
        };
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        // Act
        var result = await _service.RejectTransaction(transaction.Id);

        // Assert
        Assert.Equal(TransactionStatus.Rejected, result.Status);
    }

    [Fact]
    public async Task GetPendingTransactionsCount_ReturnsCorrectCount()
    {
        // Arrange
        _db.Transactions.AddRange(
            new Transaction { Status = TransactionStatus.Pending },
            new Transaction { Status = TransactionStatus.Accepted }
        );
        await _db.SaveChangesAsync();

        // Act
        var count = await _service.GetPendingTransactionsCount();

        // Assert
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetUserBalance_CalculatesCorrectly()
    {
        // Arrange
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

        // Act
        var balance = await _service.GetUserBalance(userId);

        // Assert
        Assert.Equal(60, balance);
    }
}

// Helper class to provide Sieve options
public class SieveOptionsAccessor : Microsoft.Extensions.Options.IOptions<SieveOptions>
{
    public SieveOptions Value => new SieveOptions();
}