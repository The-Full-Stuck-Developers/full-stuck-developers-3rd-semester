using api.Models.Dtos.Requests.Transaction;
using dataccess;
using Dtos;

namespace api.Services;

public class TransactionService(MyDbContext dbContext) : ITransactionService
{
    public Task<List<TransactionDto>> GetAllTransactions()
    {
        throw new NotImplementedException();
    }

    public Task<TransactionDto?> GetTransactionById(string id)
    {
        throw new NotImplementedException();
    }

    public Task<TransactionDto> CreateTransaction(CreateTransactionDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<TransactionDto?> UpdateTransactionStatus(string id, UpdateTransactionDto dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTransaction(string id)
    {
        throw new NotImplementedException();
    }
}