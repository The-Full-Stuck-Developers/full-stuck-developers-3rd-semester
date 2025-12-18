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
}