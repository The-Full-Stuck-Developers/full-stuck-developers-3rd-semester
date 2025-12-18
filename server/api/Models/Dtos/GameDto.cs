using dataccess;
using DefaultNamespace;

namespace Dtos;

public class GameDto
{
    public GameDto(Game game)
    {
        Id = game.Id;
        WeekNumber = game.WeekNumber;
        Year = game.Year;
        StartTime = game.StartTime;
        BetDeadline = game.BetDeadline;
        DrawDate = game.DrawDate;
        WinningNumbers = game.WinningNumbers;
        IsDrawn = game.IsDrawn;
        CanBet = game.CanBet;
        Bets = game.Bets;
        InPersonWinners = game.InPersonWinners;
        InPersonPrizePool = game.InPersonPrizePool;
    }

    public Guid Id { get; set; }
    public int WeekNumber { get; set; }
    public int Year { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime BetDeadline { get; set; }
    public DateTime? DrawDate { get; set; }
    public int Revenue => Bets.Sum(b => b.Transaction.Amount);

    public string? WinningNumbers { get; set; }

    public bool IsDrawn { get; set; }
    public bool CanBet { get; set; }
    public ICollection<Bet> Bets { get; set; }

    public int? InPersonWinners { get; set; }

    public int? InPersonPrizePool { get; set; }
}
