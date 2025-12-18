using dataccess.Entities;
using DefaultNamespace;

namespace dataccess;

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public int Amount { get; set; }
    public int? MobilePayTransactionNumber { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    public TransactionType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Bet Bet { get; set; } = null!;
}
