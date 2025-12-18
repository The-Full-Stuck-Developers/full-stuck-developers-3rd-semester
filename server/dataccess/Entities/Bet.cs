using System.ComponentModel.DataAnnotations;
using dataccess;
using dataccess.Entities;

namespace DefaultNamespace;

public class Bet
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
    public Guid TransactionId { get; set; }
    public required Transaction Transaction { get; set; }
    public string SelectedNumbers { get; set; } = null!;
    public bool IsWinning { get; set; }

    public int Winnings { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
