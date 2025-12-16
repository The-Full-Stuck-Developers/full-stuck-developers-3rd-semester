using api.Models;
using api.Models.Dtos.Requests.Transaction;
using Dtos;
using Sieve.Models;

namespace api.Services;

public interface ITransactionService
{
    Task<PagedResult<TransactionDto>> GetAllTransactions(SieveModel sieveModel);
    Task<PagedResult<TransactionDto>> GetTransactionsByUser(Guid userId, SieveModel sieveModel);
    Task<TransactionDto?> GetTransactionById(Guid id);
    Task<TransactionDto> CreateTransaction(CreateTransactionDto dto);
    Task<TransactionDto?> UpdateTransactionStatus(Guid id, UpdateTransactionDto dto);
    Task DeleteTransaction(Guid id);
    Task<TransactionDto> ApproveTransaction(Guid id);
    Task<TransactionDto> RejectTransaction(Guid id);
    Task<int> GetPendingTransactionsCount();
    Task<int> GetUserBalance(Guid userId);
    Task<int> GetUserPurchaseTotal(Guid userId);
    Task<int> GetUserDepositTotal(Guid userId);
}
