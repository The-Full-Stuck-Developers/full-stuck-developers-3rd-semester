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

// {
// Task<Game> GetCurrentGameAsync();
// Task<Game> GetGameByIdAsync(Guid gameId);
// Task<Game> DrawWinningNumbersAsync(Guid gameId, string winningNumbersCsv, Guid adminId);
// Task<List<Bet>> GetDigitalWinningBetsAsync(Guid gameId);
// Task<int> GetTotalWinningBetsCountAsync(Guid gameId, int physicalWinnersCount);
// Task SeedFutureGamesAsync(int yearsAhead = 40);
// Task<int> CalculateGameRevenueAsync(Guid gameId);
// }