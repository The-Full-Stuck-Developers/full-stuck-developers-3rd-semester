using api.Models.Dtos.Requests.Transaction;
using Dtos;

namespace api.Services;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetAllTransactions();
    Task<TransactionDto?> GetTransactionById(string id);
    Task<TransactionDto> CreateTransaction(CreateTransactionDto dto);
    Task<TransactionDto?> UpdateTransactionStatus(string id, UpdateTransactionDto dto);
    Task DeleteTransaction(string id);
}
