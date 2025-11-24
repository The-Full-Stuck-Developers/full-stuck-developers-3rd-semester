using dataccess;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class GameService : IGameService
{
    private readonly MyDbContext _db;
    
    public GameService(MyDbContext db)
    {
        _db = db;
    }

    public Task<Game> GetCurrentGameAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Game> GetOrCreateCurrentGameAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Game> DrawWinningNumbersAsync(Guid gameId, string winningNumbersCsv, Guid adminId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Bet>> GetDigitalWinningBetsAsync(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetAllWinningBetsAsync(Guid gameId)
    {
        throw new NotImplementedException();
    }

    public Task SeedFutureGamesIfNeededAsync(int yearsAhead = 20)
    {
        throw new NotImplementedException();
    }
}