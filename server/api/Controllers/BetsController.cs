using System.Security.Claims;
using api.Services;
using dataccess;
using dataccess.Entities;
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
    private readonly ILogger<BetsController> _logger;

    public BetsController(MyDbContext db, ITransactionService transactionService, IBoardService boardService, ILogger<BetsController> logger)
    {
        _db = db;
        _transactionService = transactionService;
        _boardService = boardService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<PlaceBetResponse>> PlaceBet([FromBody] CreateBetDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        int totalCost = dto.Price * dto.RepeatWeeks;

        int balance = await _transactionService.GetUserBalance(userId);
        if (balance < totalCost)
            return BadRequest(new PlaceBetResponse(false, "Not enough money", Guid.Empty, "", 0, 0, DateTime.UtcNow));

        // Use a database transaction to ensure both operations succeed or fail together
        using var dbTransaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var purchase = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = totalCost,
                Type = TransactionType.Purchase,
                Status = TransactionStatus.Accepted,
                CreatedAt = DateTime.UtcNow
            };
            _db.Transactions.Add(purchase);
            await _db.SaveChangesAsync();

            var betId = await _boardService.CreateBet(dto, userId, purchase);
            var bet = await _db.Bets
                .Where(b => b.Id == betId)
                .FirstAsync();

            await dbTransaction.CommitAsync();

            string sortedNumbers = bet.SelectedNumbers;

            return Ok(new PlaceBetResponse(
                Success: true,
                Message: $"Bet {totalCost} kr",
                BetId: bet.Id,
                SortedNumbers: sortedNumbers,
                Count: dto.Count,
                Price: totalCost,
                CreatedAt: bet.CreatedAt
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
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var bets = rawBets.Select(b => new BetHistoryDto(
            b.Id,
            b.SelectedNumbers,
            b.SelectedNumbers.Split(',').Length,
            b.Transaction.Amount,
            b.CreatedAt
        )).ToList();

        int total = await _db.Bets.CountAsync(b => b.UserId == userId);

        return Ok(new BetHistoryResponse(bets, total, page, pageSize));
    }
}