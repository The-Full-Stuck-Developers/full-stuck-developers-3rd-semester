using api.Models.Dtos.Requests.Transaction;
using Dtos;

namespace api.Services;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetAllTransactions();
    Task<TransactionDto?> GetTransactionById(Guid id);
    Task<TransactionDto> CreateTransaction(CreateTransactionDto dto);
    Task<TransactionDto?> UpdateTransactionStatus(Guid id, UpdateTransactionDto dto);
    Task DeleteTransaction(Guid id);
}
