using System.ComponentModel.DataAnnotations;
using api.Models;
using api.Models.Dtos.Requests.Transaction;
using api.Services;
using Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController(ITransactionService transactionService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetAllTransactions([FromQuery] SieveModel sieveModel)
    {
        var result = await transactionService.GetAllTransactions(sieveModel);

        return Ok(result);
    }

    [HttpGet("User/{userId}")]
    public async Task<ActionResult<PagedResult<TransactionDto>>> GetTransactionsByUser(
        Guid userId,
        [FromQuery] SieveModel sieveModel)
    {
        var result = await transactionService.GetTransactionsByUser(userId, sieveModel);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<TransactionDto>> GetTransactionById(Guid id)
    {
        var transaction = await transactionService.GetTransactionById(id);
        if (transaction == null)
            return NotFound();

        return Ok(transaction);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransactionDto>> CreateTransaction(
        [FromBody] CreateTransactionDto createTransactionDto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var created = await transactionService.CreateTransaction(createTransactionDto);
            return CreatedAtAction(
                nameof(GetTransactionById),
                new { id = created.Id },
                created
            );
        }
        catch (ValidationException ex)
        {
            // From Validator.ValidateObject in service
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // In case you later add domain checks that throw this
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "IsAdmin")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransactionDto>> UpdateTransactionStatus(
        Guid id,
        [FromBody] UpdateTransactionDto updateTransactionDto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var updated = await transactionService.UpdateTransactionStatus(id, updateTransactionDto);
            return Ok(updated);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Transaction with id {id} not found" });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        try
        {
            await transactionService.DeleteTransaction(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Transaction with id {id} not found" });
        }
    }

    [HttpPatch("{id}/approve")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<PagedResult<TransactionDto>>> ApproveTransaction(Guid id)
    {
        var transaction = await transactionService.ApproveTransaction(id);

        return Ok(transaction);
    }

    [HttpPatch("{id}/reject")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<IActionResult> RejectTransaction(Guid id)
    {
        await transactionService.RejectTransaction(id);

        return NoContent();
    }

    [HttpGet("GetPendingTransactionsCount")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<int>> GetPendingTransactionsCount()
    {
        var count = await transactionService.GetPendingTransactionsCount();

        return Ok(new { count = count });
    }

    [HttpGet("GetUserBalance")]
    // [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<int>> GetUserBalance(Guid userId)
    {
        var balance = await transactionService.GetUserBalance(userId);

        return Ok(new { balance = balance });
    }
}
