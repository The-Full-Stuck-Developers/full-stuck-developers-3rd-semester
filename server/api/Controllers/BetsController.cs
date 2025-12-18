using System.Security.Claims;
using api.Services;
using dataccess;
using dataccess.Entities;
using DefaultNamespace;
using Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BetsController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly ITransactionService _transactionService;
    private readonly IBoardService _boardService;
    private readonly IGameService _gameService;
    private readonly ILogger<BetsController> _logger;

    public BetsController(MyDbContext db, ITransactionService transactionService, IBoardService boardService, IGameService gameService, ILogger<BetsController> logger)
    {
        _db = db;
        _transactionService = transactionService;
        _boardService = boardService;
        _gameService = gameService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<PlaceBetResponse>> PlaceBet([FromBody] CreateBetDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        var user = await _db.Users.FindAsync(userId);
        if (user == null || !user.IsActive)
        {
            return BadRequest(new PlaceBetResponse(false, "Your account is inactive. Please contact an administrator.", Guid.Empty, "", 0, 0, DateTime.UtcNow));
        }

        int totalCost = dto.Price * dto.RepeatWeeks;

        int balance = await _transactionService.GetUserBalance(userId);
        if (balance < totalCost)
        {
            return BadRequest(new PlaceBetResponse(false, "Top up your account", Guid.Empty, "", 0, 0, DateTime.UtcNow));
        }

        using var dbTransaction = await _db.Database.BeginTransactionAsync();
        try
        {
            List<Guid> betIds;
            
            if (dto.RepeatWeeks > 1)
            {
                betIds = await _boardService.CreateBetsForWeeks(dto, userId, dto.RepeatWeeks);
            }
            else
            {
                var purchase = new Transaction
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Amount = dto.Price,
                    Type = TransactionType.Purchase,
                    Status = TransactionStatus.Accepted,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Transactions.Add(purchase);
                await _db.SaveChangesAsync();

                var betId = await _boardService.CreateBet(dto, userId, purchase);
                betIds = new List<Guid> { betId };
            }

            await dbTransaction.CommitAsync();

            var firstBet = await _db.Bets
                .Where(b => b.Id == betIds[0])
                .FirstAsync();

            string sortedNumbers = firstBet.SelectedNumbers;
            string message = dto.RepeatWeeks > 1 
                ? $"Created {dto.RepeatWeeks} bets for {totalCost} kr" 
                : $"Bet {dto.Price} kr";

            return Ok(new PlaceBetResponse(
                Success: true,
                Message: message,
                BetId: firstBet.Id,
                SortedNumbers: sortedNumbers,
                Count: dto.Count,
                Price: totalCost,
                CreatedAt: firstBet.CreatedAt
            ));
        }
        catch (InvalidOperationException ex)
        {
            await dbTransaction.RollbackAsync();
            _logger.LogError(ex, "InvalidOperationException while placing bet for user {UserId}", userId);
            
            return BadRequest(new PlaceBetResponse(
                false, 
                ex.Message.Contains("No future games") 
                    ? "No games available for betting. Please contact an administrator." 
                    : "Unable to create bet. Please try again.",
                Guid.Empty, 
                "", 
                0, 
                0, 
                DateTime.UtcNow
            ));
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync();
            _logger.LogError(ex, "Exception while placing bet for user {UserId}: {Message}", userId, ex.Message);
            
            return BadRequest(new PlaceBetResponse(
                false, 
                $"An error occurred while placing your bet: {ex.Message}",
                Guid.Empty, 
                "", 
                0, 
                0, 
                DateTime.UtcNow
            ));
        }
    }

    [HttpGet("player/history")]
    public async Task<ActionResult<BetHistoryResponse>> GetUserHistory(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var rawBets = await _db.Bets
            .Include(b => b.Transaction)
            .Include(b => b.Game)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var seriesIds = rawBets.Where(b => b.BetSeriesId.HasValue).Select(b => b.BetSeriesId!.Value).Distinct().ToList();
        var seriesCounts = new Dictionary<Guid, int>();
        var seriesBetOrders = new Dictionary<Guid, List<Guid>>();
        
        foreach (var seriesId in seriesIds)
        {
            seriesCounts[seriesId] = await _db.Bets
                .Where(b => b.BetSeriesId == seriesId && b.UserId == userId)
                .CountAsync();
            
            var seriesBets = await _db.Bets
                .Include(bet => bet.Game)
                .Where(bet => bet.BetSeriesId == seriesId && bet.UserId == userId)
                .OrderBy(bet => bet.Game.StartTime)
                .Select(bet => bet.Id)
                .ToListAsync();
            seriesBetOrders[seriesId] = seriesBets;
        }
        
        var bets = rawBets.Select(b => 
        {
            int? seriesTotal = null;
            int? seriesIndex = null;
            
            if (b.BetSeriesId.HasValue && seriesCounts.ContainsKey(b.BetSeriesId.Value))
            {
                seriesTotal = seriesCounts[b.BetSeriesId.Value];
                var seriesBets = seriesBetOrders[b.BetSeriesId.Value];
                seriesIndex = seriesBets.IndexOf(b.Id) + 1;
            }
            
            return new BetHistoryDto(
                b.Id,
                b.SelectedNumbers,
                b.SelectedNumbers.Split(',').Length,
                b.Transaction.Amount,
                b.CreatedAt,
                b.IsWinning,
                b.BetSeriesId,
                seriesTotal,
                seriesIndex,
                b.Game?.WeekNumber,
                b.Game?.Year,
                b.Game?.StartTime
            );
        }).ToList();

        int total = await _db.Bets
            .Where(b => b.UserId == userId)
            .CountAsync();

        return Ok(new BetHistoryResponse(bets, total, page, pageSize));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBet(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        var bet = await _db.Bets
            .Include(b => b.Transaction)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (bet == null)
            return NotFound();

        var game = await _db.Games.FirstOrDefaultAsync(g => g.Id == bet.GameId);
        if (game != null && game.WinningNumbers != null)
        {
            return BadRequest(new { message = "Cannot delete bet for a game that has already been drawn" });
        }

        using var dbTransaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var refund = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = bet.Transaction.Amount,
                Type = TransactionType.Refund,
                Status = TransactionStatus.Accepted,
                CreatedAt = DateTime.UtcNow
            };
            _db.Transactions.Add(refund);

            _db.Bets.Remove(bet);
            
            await _db.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync();
            _logger.LogError(ex, "Error deleting bet {BetId} for user {UserId}", id, userId);
            return StatusCode(500, new { message = "An error occurred while deleting the bet" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBet(Guid id, [FromBody] UpdateBetDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        // Check if user is active
        var user = await _db.Users.FindAsync(userId);
        if (user == null || !user.IsActive)
        {
            return BadRequest(new { message = "Your account is inactive. Please contact an administrator." });
        }

        var bet = await _db.Bets
            .Include(b => b.Transaction)
            .Include(b => b.Game)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (bet == null)
            return NotFound();

        var game = bet.Game;
        if (game == null)
            return BadRequest(new { message = "Game not found for this bet" });

        if (game.WinningNumbers != null)
        {
            return BadRequest(new { message = "Cannot edit bet for a game that has already been drawn" });
        }

        if (game.BetDeadline < DateTime.UtcNow)
        {
            return BadRequest(new { message = "Cannot edit bet after game deadline" });
        }

        if (bet.DeletedAt != null)
        {
            return BadRequest(new { message = "Cannot edit deleted bet" });
        }

        if (dto.Numbers == null || dto.Numbers.Count < 5 || dto.Numbers.Count > 8)
        {
            return BadRequest(new { message = "Bet must have between 5 and 8 numbers" });
        }

        var oldCount = bet.SelectedNumbers.Split(',').Length;
        var newCount = dto.Numbers.Count;
        
        var costMap = new Dictionary<int, int>
        {
            { 5, 20 },
            { 6, 40 },
            { 7, 80 },
            { 8, 160 }
        };

        var oldPrice = bet.Transaction.Amount;
        var newPrice = costMap[newCount];

        using var dbTransaction = await _db.Database.BeginTransactionAsync();
        try
        {
            if (newPrice != oldPrice)
            {
                var priceDifference = newPrice - oldPrice;
                
                if (priceDifference > 0)
                {
                    var balance = await _transactionService.GetUserBalance(userId);
                    if (balance < priceDifference)
                    {
                        return BadRequest(new { message = "Insufficient balance to update bet" });
                    }

                    var additionalPurchase = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Amount = priceDifference,
                        Type = TransactionType.Purchase,
                        Status = TransactionStatus.Accepted,
                        CreatedAt = DateTime.UtcNow
                    };
                    _db.Transactions.Add(additionalPurchase);
                }
                else
                {
                    var refund = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        Amount = Math.Abs(priceDifference),
                        Type = TransactionType.Refund,
                        Status = TransactionStatus.Accepted,
                        CreatedAt = DateTime.UtcNow
                    };
                    _db.Transactions.Add(refund);
                }

                bet.Transaction.Amount = newPrice;
            }

            bet.SelectedNumbers = string.Join(",", dto.Numbers.OrderBy(n => n));
            
            await _db.SaveChangesAsync();
            await dbTransaction.CommitAsync();

            return Ok(new { message = "Bet updated successfully" });
        }
        catch (Exception ex)
        {
            await dbTransaction.RollbackAsync();
            _logger.LogError(ex, "Error updating bet {BetId} for user {UserId}", id, userId);
            return StatusCode(500, new { message = "An error occurred while updating the bet" });
        }
    }

    [HttpGet("player/active")]
    public async Task<ActionResult<BetHistoryResponse>> GetUserActiveBoards(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var now = DateTime.UtcNow;
        
        var rawBets = await _db.Bets
            .Include(b => b.Transaction)
            .Include(b => b.Game)
            .Where(b => b.UserId == userId 
                && b.DeletedAt == null
                && b.Game.WinningNumbers == null 
                && b.Game.BetDeadline > now)
            .OrderBy(b => b.Game.StartTime)
            .ThenByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var seriesIds = rawBets
            .Where(b => b.BetSeriesId.HasValue)
            .Select(b => b.BetSeriesId!.Value)
            .Distinct()
            .ToList();
        var seriesCounts = new Dictionary<Guid, int>();
        var seriesBetOrders = new Dictionary<Guid, List<Guid>>();
        
        foreach (var seriesId in seriesIds)
        {
            seriesCounts[seriesId] = await _db.Bets
                .Where(b => b.BetSeriesId == seriesId && b.UserId == userId)
                .CountAsync();
            
            var seriesBets = await _db.Bets
                .Include(bet => bet.Game)
                .Where(bet => bet.BetSeriesId == seriesId && bet.UserId == userId)
                .OrderBy(bet => bet.Game.StartTime)
                .Select(bet => bet.Id)
                .ToListAsync();
            seriesBetOrders[seriesId] = seriesBets;
        }
        
        var bets = rawBets.Select(b => 
        {
            int? seriesTotal = null;
            int? seriesIndex = null;
            
            if (b.BetSeriesId.HasValue && seriesCounts.ContainsKey(b.BetSeriesId.Value))
            {
                seriesTotal = seriesCounts[b.BetSeriesId.Value];
                var seriesBets = seriesBetOrders[b.BetSeriesId.Value];
                seriesIndex = seriesBets.IndexOf(b.Id) + 1;
            }
            
            return new BetHistoryDto(
                b.Id,
                b.SelectedNumbers,
                b.SelectedNumbers.Split(',').Length,
                b.Transaction.Amount,
                b.CreatedAt,
                b.IsWinning,
                b.BetSeriesId,
                seriesTotal,
                seriesIndex,
                b.Game?.WeekNumber,
                b.Game?.Year,
                b.Game?.StartTime
            );
        }).ToList();

        int total = await _db.Bets
            .Include(b => b.Game)
            .Where(b => b.UserId == userId 
                && b.DeletedAt == null
                && b.Game.WinningNumbers == null 
                && b.Game.BetDeadline > now)
            .CountAsync();

        return Ok(new BetHistoryResponse(bets, total, page, pageSize));
    }
}