using System.Globalization;
using api.Models;
using api.Models.Dtos.Requests.Game;
using dataccess;
using dataccess.Repositories;
using DefaultNamespace;
using domain;
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
            .Include(g => g.Bets)
            .ThenInclude(b => b.Transaction)
            .Include(g => g.Bets)
            .ThenInclude(b => b.User)
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
            .Include(g => g.Bets)
            .ThenInclude(b => b.Transaction)
            .Include(g => g.Bets)
            .ThenInclude(b => b.User)
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
            .Include(g => g.Bets)
            .ThenInclude(b => b.Transaction)
            .Include(g => g.Bets)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(u => u.Id == id);

        return game == null ? null : new GameDto(game);
    }

    public async Task<Game> GetCurrentGame()
    {
        var now = DateTime.UtcNow;
        var currentYear = now.Year;
        var currentWeek = ISOWeek.GetWeekOfYear(now);

        var game = await gameRepository
            .Query()
            .Include(g => g.Bets)
            .ThenInclude(b => b.Transaction)
            .Include(g => g.Bets)
            .ThenInclude(b => b.User)
            .FirstOrDefaultAsync(g =>
                g.Year == currentYear &&
                g.WeekNumber == currentWeek);

        return game ?? throw new KeyNotFoundException(
            $"No game found for year {currentYear}, week {currentWeek}");
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

    public async Task<GameDto> UpdateWinningNumbers(Guid gameId, WinningNumbersDto winningNumbersDto)
    {
        var game = await gameRepository
            .Query()
            .Where(g => g.Id == gameId)
            .FirstOrDefaultAsync();

        if (game == null)
        {
            throw new KeyNotFoundException("Game not found");
        }

        game.WinningNumbers = new GameWinningNumbers(winningNumbersDto.WinningNumbers).Value;

        await gameRepository.UpdateAsync(game);

        return new GameDto(game);
    }

    public async Task<GameDto> DrawWinners(Guid id)
    {
        var game = await gameRepository.Query()
            .Include(g => g.Bets)
            .ThenInclude(b => b.Transaction)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null)
        {
            throw new KeyNotFoundException("Game not found");
        }

        if (string.IsNullOrWhiteSpace(game.WinningNumbers))
        {
            throw new InvalidOperationException("Game does not have winning numbers set. Please set winning numbers before drawing winners.");
        }

        var winningNumbers = new GameWinningNumbers(game.WinningNumbers);

        foreach (var bet in game.Bets)
        {
            if (!string.IsNullOrWhiteSpace(bet.SelectedNumbers))
            {
                var selectedNumbers = new GameWinningNumbers(bet.SelectedNumbers);
                bet.IsWinning = winningNumbers.IsGuessedBy(selectedNumbers);
            }
            else
            {
                bet.IsWinning = false;
            }
        }

        game.DrawDate = DateTime.UtcNow;

        await gameRepository.UpdateAsync(game);

        return new GameDto(game);
    }

    public async Task<GameDto> UpdateNumberOfPhysicalPlayers(Guid id,NumberOfPhysicalPlayersDto dto)
    {
        var game = await  gameRepository.Query()
            .Where(g => g.Id == id)
            .FirstOrDefaultAsync();

        game.NumberOfPhysicalPlayers = dto.NumberOfPhysicalPlayers;
        await gameRepository.UpdateAsync(game);

        return new GameDto(game);
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
