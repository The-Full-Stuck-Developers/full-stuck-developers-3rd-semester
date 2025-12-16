using System.Globalization;
using api.Models;
using dataccess;
using dataccess.Repositories;
using DefaultNamespace;
using Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace api.Services;

public class GameService(
    IRepository<Game> gameRepository,
    ISieveProcessor sieveProcessor
) : IGameService
{
    public async Task<PagedResult<GameDto>> GetAllUpcomingGames(SieveModel sieveModel)
    {
        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentWeek = ISOWeek.GetWeekOfYear(now);

        var query = gameRepository.Query()
            .Where(g =>
                g.Year > currentYear ||
                (g.Year == currentYear && g.WeekNumber >= currentWeek))
            .OrderBy(g => g.Year)
            .ThenBy(g => g.WeekNumber);

        var filteredQuery = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var total = await filteredQuery.CountAsync();

        var pagedQuery = sieveProcessor.Apply(sieveModel, query, applyPagination: true);
        var items = await pagedQuery.ToListAsync();

        return new PagedResult<GameDto>
        {
            Items = items.Select(g => new GameDto(g)).ToList(),
            Total = total,
            PageSize = sieveModel.PageSize ?? 10,
            PageNumber = sieveModel.Page ?? 1
        };
    }

    public async Task<PagedResult<GameDto>> GetAllPastGames(SieveModel sieveModel)
    {
        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentWeek = ISOWeek.GetWeekOfYear(now);

        var query = gameRepository.Query()
            .Where(g =>
                g.Year < currentYear ||
                (g.Year == currentYear && g.WeekNumber < currentWeek))
            .OrderByDescending(g => g.Year)
            .ThenByDescending(g => g.WeekNumber);

        var filteredQuery = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var total = await filteredQuery.CountAsync();

        var pagedQuery = sieveProcessor.Apply(sieveModel, query, applyPagination: true);
        var items = await pagedQuery.ToListAsync();

        return new PagedResult<GameDto>
        {
            Items = items.Select(g => new GameDto(g)).ToList(),
            Total = total,
            PageSize = sieveModel.PageSize ?? 10,
            PageNumber = sieveModel.Page ?? 1
        };
    }


    public async Task<GameDto?> GetGameById(Guid id)
    {
        var game = await gameRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == id);

        return game == null ? null : new GameDto(game);
    }

    public async Task<Game> GetCurrentGameAsync(Guid gameId)
    {
        var game = await gameRepository
            .Query()
            .Include(g => g.Bets)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(g => g.Id == gameId);

        return game ?? throw new KeyNotFoundException($"Game {gameId} not found");
    }

    public async Task<Game> GetOrCreateCurrentGameAsync()
    {
        var currentGame = await gameRepository
            .Query()
            .Include(g => g.Bets)
            .FirstOrDefaultAsync(g => g.CanBet);

        if (currentGame != null)
            return currentGame;


        var now = DateTime.UtcNow;
        var nextGame = await gameRepository
            .Query()
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
