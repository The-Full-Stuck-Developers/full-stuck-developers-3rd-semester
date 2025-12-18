using api.Models;
using Dtos;
using dataccess;
using dataccess.Entities;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class BoardService(MyDbContext dbContext, IGameService gameService) : IBoardService
{
    public async Task<PagedResult<BoardDto>> GetAllBoards()
    {
        var bets = await dbContext.Bets
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return new PagedResult<BoardDto>
        {
            Items = bets.Select(b => new BoardDto(b.SelectedNumbers.Split(',').Select(int.Parse).ToList())).ToList(),
            Total = bets.Count,
            PageSize = bets.Count,
            PageNumber = 1
        };
    }

    public async Task<PagedResult<BoardDto>> GetBoardsByUser(Guid userId)
    {
        var bets = await dbContext.Bets
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return new PagedResult<BoardDto>
        {
            Items = bets.Select(b => new BoardDto(b.SelectedNumbers.Split(',').Select(int.Parse).ToList())).ToList(),
            Total = bets.Count,
            PageSize = bets.Count,
            PageNumber = 1
        };
    }

    public async Task<Guid> CreateBet(CreateBetDto dto, Guid userId, Transaction transaction)
    {
        var currentGame = await gameService.GetOrCreateCurrentGameAsync();
        
        var bet = new Bet
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GameId = currentGame.Id,
            TransactionId = transaction.Id,
            Transaction = transaction,
            SelectedNumbers = string.Join(",", dto.Numbers.OrderBy(n => n)),
            CreatedAt = DateTime.UtcNow
        };

        dbContext.Bets.Add(bet);
        await dbContext.SaveChangesAsync();
        
        return bet.Id;
    }

    public async Task<List<Guid>> CreateBetsForWeeks(CreateBetDto dto, Guid userId, int repeatWeeks)
    {
        // Generate a series ID to link all bets together
        var seriesId = Guid.NewGuid();
        var betIds = new List<Guid>();
        
        // Get or create games for the next N weeks
        var games = await gameService.GetOrCreateGamesForWeeksAsync(repeatWeeks);
        
        if (games.Count != repeatWeeks)
        {
            throw new InvalidOperationException($"Could not create or find {repeatWeeks} games");
        }
        
        // Create a bet for each game with its own transaction
        foreach (var game in games)
        {
            // Create individual transaction for each bet
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = dto.Price, // Individual bet price, not total
                Type = TransactionType.Purchase,
                Status = TransactionStatus.Accepted,
                CreatedAt = DateTime.UtcNow
            };
            
            dbContext.Transactions.Add(transaction);
            
            var bet = new Bet
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                GameId = game.Id,
                TransactionId = transaction.Id,
                Transaction = transaction,
                SelectedNumbers = string.Join(",", dto.Numbers.OrderBy(n => n)),
                CreatedAt = DateTime.UtcNow,
                BetSeriesId = seriesId
            };
            
            dbContext.Bets.Add(bet);
            betIds.Add(bet.Id);
        }
        
        await dbContext.SaveChangesAsync();
        
        return betIds;
    }
}