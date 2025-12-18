using System.ComponentModel.DataAnnotations;
using DefaultNamespace;

namespace dataccess;

public class Game
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public int WeekNumber { get; set; }
    public int Year { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime BetDeadline { get; set; }
    public DateTime? DrawDate { get; set; }
    public int Revenue => Bets.Sum(b => b.Transaction.Amount);
    public string? WinningNumbers { get; set; }
    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    public bool IsDrawn => WinningNumbers != null;
    public bool CanBet => WinningNumbers == null && DateTime.UtcNow < BetDeadline;

    public int? NumberOfPhysicalPlayers { get; set; }
}
