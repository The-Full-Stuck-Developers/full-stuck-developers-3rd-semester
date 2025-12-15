using System.ComponentModel.DataAnnotations;
using api.Models;
using api.Models.Dtos.Requests.Transaction;
using dataccess;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace api.Services;

public class TransactionService(MyDbContext dbContext, ISieveProcessor sieveProcessor) : ITransactionService
{
    public async Task<PagedResult<TransactionDto>> GetAllTransactions(SieveModel sieveModel)
    {
        var query = dbContext.Transactions
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt);

        var filteredQuery = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var total = await filteredQuery.CountAsync();
        var result = sieveProcessor.Apply(sieveModel, query);
        var items = await result.ToListAsync();
        var transactions = items.Select(t => new TransactionDto(t)).ToList();

        return new PagedResult<TransactionDto>
        {
            Items = transactions,
            Total = total,
            PageSize = sieveModel.PageSize ?? 10,
            PageNumber = sieveModel.Page ?? 1
        };
    }

    public async Task<PagedResult<TransactionDto>> GetTransactionsByUser(Guid userId, SieveModel sieveModel)
    {
        var query = dbContext.Transactions
            .Include(t => t.User)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt);

        var filteredQuery = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var total = await filteredQuery.CountAsync();
        var result = sieveProcessor.Apply(sieveModel, query);
        var items = await result.ToListAsync();
        var transactions = items.Select(t => new TransactionDto(t)).ToList();

        return new PagedResult<TransactionDto>
        {
            Items = transactions,
            Total = total,
            PageSize = sieveModel.PageSize ?? 10,
            PageNumber = sieveModel.Page ?? 1
        };
    }

    public Task<TransactionDto?> GetTransactionById(Guid id)
    {
        return dbContext.Transactions.Where(t => t.Id == id).Select(t => new TransactionDto(t)).FirstOrDefaultAsync();
    }

    public async Task<TransactionDto> CreateTransaction(CreateTransactionDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse(dto.UserId),
            Amount = dto.Amount,
            MobilePayTransactionNumber = dto.MobilePayTransactionNumber,
            Status = TransactionStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync();
        return new TransactionDto(transaction);
    }

    public async Task<TransactionDto?> UpdateTransactionStatus(Guid id, UpdateTransactionDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var transaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if (transaction == null) throw new KeyNotFoundException($"Transaction {id} not found");

        transaction.Status = dto.Status;

        await dbContext.SaveChangesAsync();
        return new TransactionDto(transaction);
    }

    public async Task DeleteTransaction(Guid id)
    {
        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            throw new KeyNotFoundException($"Transaction {id} not found");

        transaction.Status = TransactionStatus.Cancelled;

        await dbContext.SaveChangesAsync();
    }

    public async Task<TransactionDto> ApproveTransaction(Guid id)
    {
        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null)
            throw new KeyNotFoundException($"Transaction not found");

        transaction.Status = TransactionStatus.Accepted;

        // Explicitly mark as modified
        dbContext.Entry(transaction).State = EntityState.Modified;

        await dbContext.SaveChangesAsync();

        return new TransactionDto(transaction);
    }

    public async Task RejectTransaction(Guid id)
    {
        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null) throw new KeyNotFoundException($"Transaction not found");

        transaction.Status = TransactionStatus.Rejected;
        await dbContext.SaveChangesAsync();
    }

    public async Task<int> GetPendingTransactionsCount()
    {
        return await dbContext.Transactions
            .Where(t => t.Status == TransactionStatus.Pending)
            .CountAsync();
    }

    public async Task<int> GetUserBalance(Guid userId)
    {
        return await  dbContext.Transactions
            .Where(t => t.UserId == userId)
            .Where(t => t.Status == TransactionStatus.Accepted)
            .SumAsync(t => t.Amount);
    }
}
