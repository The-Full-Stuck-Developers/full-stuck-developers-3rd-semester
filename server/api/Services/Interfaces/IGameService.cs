using api.Models;
using api.Models.Dtos.Requests.Game;
using dataccess;
using DefaultNamespace;
using Dtos;
using Sieve.Models;

namespace api.Services;

public interface IGameService
{
    Task<PagedResult<GameDto>> GetAllUpcomingGames(SieveModel sieveModel);
    Task<PagedResult<GameDto>> GetAllPastGames(SieveModel sieveModel);
    Task<GameDto?> GetGameById(Guid id);
    Task<Game> GetCurrentGame();
    Task<Game> GetOrCreateCurrentGameAsync();
    Task<GameDto> UpdateWinningNumbers(Guid gameId, WinningNumbersDto winningNumbers);
    Task<GameDto> DrawWinners(Guid id);
    Task<GameDto> UpdateInPersonData(Guid id, InPersonDto dto);
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
