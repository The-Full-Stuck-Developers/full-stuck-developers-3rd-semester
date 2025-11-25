using System.ComponentModel.DataAnnotations;
using api.Models.Dtos.Requests.Transaction;
using dataccess;
using Dtos;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class TransactionService(MyDbContext dbContext) : ITransactionService
{
    public Task<List<TransactionDto>> GetAllTransactions()
    {
        return dbContext.Transactions.Select(t => new TransactionDto(t)).ToListAsync();
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
        if(transaction == null) throw new KeyNotFoundException($"Transaction {id} not found");
        
        transaction.Status = dto.Status;
        
        await dbContext.SaveChangesAsync();
        return new TransactionDto(transaction);
    }

    public async Task DeleteTransaction(Guid id)
    {
        var transaction = await dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
        if(transaction == null) throw new KeyNotFoundException($"Transaction {id} not found");

        dbContext.Remove(transaction);
        await dbContext.SaveChangesAsync();
        return;
    }
}