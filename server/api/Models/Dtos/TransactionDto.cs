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
        Type = entity.Type;
        CreatedAt = entity.CreatedAt;
        User = entity.User;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Amount { get; set; }
    public int? MobilePayTransactionNumber { get; set; }
    public TransactionStatus Status { get; set; }
    public TransactionType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; }
}
