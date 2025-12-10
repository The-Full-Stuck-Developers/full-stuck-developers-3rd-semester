using dataccess;
using DefaultNamespace;

namespace api.Services;

public interface IGameService
{
    Task<Game> GetCurrentGameAsync(Guid gameId);                    
    Task<Game> GetOrCreateCurrentGameAsync();           
    Task<Game> DrawWinningNumbersAsync(Guid gameId, string winningNumbersCsv, Guid adminId);
    Task<List<Bet>> GetDigitalWinningBetsAsync(Guid gameId);    
    Task<int> GetAllWinningBetsAsync(Guid gameId);     
    Task SeedFutureGamesIfNeededAsync(int yearsAhead = 20);
}