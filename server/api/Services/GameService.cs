using System.Globalization;
using dataccess;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class GameService(MyDbContext dbContext) : IGameService
{
    /*
    private readonly MyDbContext _db;
    
    public GameService(MyDbContext db)
    {
        _db = db;
    }
    */

    public async Task<Game> GetCurrentGameAsync(Guid gameId)
    {
        var game = await dbContext.Games
            .Include(g => g.Bets)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(g => g.Id == gameId);
        
        return game ?? throw new KeyNotFoundException($"Game {gameId} not found");
    }

    public async Task<Game> GetOrCreateCurrentGameAsync()
    {
        var currentGame = await dbContext.Games
            .Include(g => g.Bets)
            .FirstOrDefaultAsync(g => g.CanBet);
        
        if (currentGame != null)
            return currentGame;
        

        var now = DateTime.UtcNow;
        var nextGame = await dbContext.Games
            .Where(g => g.WinningNumbers == null && g.BetDeadline > now)
            .OrderBy(g => g.StartTime)
            .FirstOrDefaultAsync();
        
        return nextGame ?? throw new InvalidOperationException("No future games available. Please seed more games.");
    }

    public Task<Game> DrawWinningNumbersAsync(Guid gameId, string winningNumbersCsv, Guid adminId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Bet>> GetDigitalWinningBetsAsync(Guid gameId)
    {
        throw new NotImplementedException();
    }


    public Task<int> GetAllWinningBetsAsync(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public async Task SeedFutureGamesIfNeededAsync(int yearsAhead = 20)
    {
        throw new NotImplementedException();
    }
}