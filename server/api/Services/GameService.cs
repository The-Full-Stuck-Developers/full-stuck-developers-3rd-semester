using System.Globalization;
using api.Models;
using api.Models.Dtos.Requests.Game;
using api.Utilities;
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
        var now = DateTime.UtcNow;
        
        // Find a game where betting is still open (inline for EF Core translation)
        var currentGame = await gameRepository
            .Query()
            .Include(g => g.Bets)
            .Where(g => g.WinningNumbers == null && g.BetDeadline > now)
            .OrderBy(g => g.StartTime)
            .FirstOrDefaultAsync();

        if (currentGame != null)
            return currentGame;

        // No game found - create one for the current week
        var currentWeek = ISOWeek.GetWeekOfYear(now);
        var currentYear = now.Year;
        
        // Get Monday of this ISO week
        var weekMonday = DateTime.SpecifyKind(
            ISOWeek.ToDateTime(currentYear, currentWeek, DayOfWeek.Monday),
            DateTimeKind.Utc
        );
        
        // Game starts Monday at 00:01
        var startTime = new DateTime(
            weekMonday.Year, weekMonday.Month, weekMonday.Day,
            0, 1, 0, DateTimeKind.Utc
        );
        
        var saturdayDeadline = weekMonday.AddDays(5);
        var betDeadline = new DateTime(
            saturdayDeadline.Year, saturdayDeadline.Month, saturdayDeadline.Day,
            17, 0, 0, DateTimeKind.Utc
        );
        
        // If we're past this week's deadline, create next week's game
        if (betDeadline <= now)
        {
            currentWeek++;
            if (currentWeek > ISOWeek.GetWeeksInYear(currentYear))
            {
                currentWeek = 1;
                currentYear++;
            }
            
            weekMonday = DateTime.SpecifyKind(
                ISOWeek.ToDateTime(currentYear, currentWeek, DayOfWeek.Monday),
                DateTimeKind.Utc
            );
            
            startTime = new DateTime(
                weekMonday.Year, weekMonday.Month, weekMonday.Day,
                0, 1, 0, DateTimeKind.Utc
            );
            
            saturdayDeadline = weekMonday.AddDays(5);
            betDeadline = new DateTime(
                saturdayDeadline.Year, saturdayDeadline.Month, saturdayDeadline.Day,
                17, 0, 0, DateTimeKind.Utc
            );
        }

        var newGame = new Game
        {
            Id = Guid.NewGuid(),
            WeekNumber = currentWeek,
            Year = currentYear,
            StartTime = startTime,
            BetDeadline = betDeadline,
            DrawDate = null,
            WinningNumbers = null
        };

        await gameRepository.AddAsync(newGame);
        
        return newGame;
    }

    public async Task<List<Game>> GetOrCreateGamesForWeeksAsync(int numberOfWeeks)
    {
        var games = new List<Game>();
        var now = DateTime.UtcNow;
        
        // Start from current week
        var currentWeek = ISOWeek.GetWeekOfYear(now);
        var currentYear = now.Year;
        
        for (int i = 0; i < numberOfWeeks; i++)
        {
            var targetWeek = currentWeek + i;
            var targetYear = currentYear;
            
            // Handle year rollover
            var weeksInYear = ISOWeek.GetWeeksInYear(targetYear);
            if (targetWeek > weeksInYear)
            {
                targetWeek -= weeksInYear;
                targetYear++;
            }
            
            // Try to find existing game
            var existingGame = await gameRepository
                .Query()
                .FirstOrDefaultAsync(g => g.Year == targetYear && g.WeekNumber == targetWeek);
            
            if (existingGame != null)
            {
                games.Add(existingGame);
            }
            else
            {
                // Create game for this week
                var weekMonday = DateTime.SpecifyKind(
                    ISOWeek.ToDateTime(targetYear, targetWeek, DayOfWeek.Monday),
                    DateTimeKind.Utc
                );
                
                var startTime = new DateTime(
                    weekMonday.Year, weekMonday.Month, weekMonday.Day,
                    0, 1, 0, DateTimeKind.Utc
                );
                
                var saturdayDeadline = weekMonday.AddDays(5);
                var betDeadline = new DateTime(
                    saturdayDeadline.Year, saturdayDeadline.Month, saturdayDeadline.Day,
                    17, 0, 0, DateTimeKind.Utc
                );
                
                var newGame = new Game
                {
                    Id = Guid.NewGuid(),
                    WeekNumber = targetWeek,
                    Year = targetYear,
                    StartTime = startTime,
                    BetDeadline = betDeadline,
                    DrawDate = null,
                    WinningNumbers = null
                };
                
                await gameRepository.AddAsync(newGame);
                games.Add(newGame);
            }
        }
        
        return games;
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
            
            // Soft delete all bets when game finishes - they should only appear in history
            if (bet.DeletedAt == null)
            {
                bet.DeletedAt = DateTime.UtcNow;
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
