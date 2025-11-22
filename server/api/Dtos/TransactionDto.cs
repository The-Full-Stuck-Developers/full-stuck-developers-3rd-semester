using dataccess;
using dataccess.Entities;

namespace Dtos;

public class TransactionDto
{
    public TransactionDto(Transaction entity)
    {
        Id = entity.Id;
        UserId = entity.UserId;
        Amount = entity.Amount;
        MobilePayTransactionNumber = entity.MobilePayTransactionNumber;
        Status = entity.Status;
        CreatedAt = entity.CreatedAt;
        User = entity.User;
    }
    
    public string Id { get; set; }
    public string UserId { get; set; }
    public int Amount { get; set; }
    public int MobilePayTransactionNumber { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; }
    
}