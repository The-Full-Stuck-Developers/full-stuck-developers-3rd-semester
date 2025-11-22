using dataccess.Entities;

namespace dataccess;

public class Transaction
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public int Amount { get; set; }
    public int MobilePayTransactionNumber { get; set; }
    public TransactionStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; }
}