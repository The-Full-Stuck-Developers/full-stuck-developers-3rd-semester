using api.Models;
using dataccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dtos;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
[AllowAnonymous]
public class GamesController(
    IRepository<Game> gameRepository,
    ISieveProcessor sieveProcessor)
    : ControllerBase
{
    [HttpGet]
    // [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<PagedResult<GameDto>>> GetAllGames([FromQuery] SieveModel sieveModel)
    {
        var query = gameRepository.Query();
        var filteredQuery = sieveProcessor.Apply(sieveModel, query);

        var total = await query.CountAsync();
        var items = await filteredQuery.ToListAsync();
        var games = items.Select(g => new GameDto(g)).ToList();

        return Ok(new PagedResult<GameDto>
        {
            Items = games,
            Total = total,
            PageSize = sieveModel.PageSize ?? 10,
            PageNumber = sieveModel.Page ?? 1
        });
    }

    [HttpGet("{id:guid}")]
    // [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<GameDto>> GetGameById(Guid id)
    {
        var game = await gameRepository
            .Query()
            .FirstOrDefaultAsync(g => g.Id == id);

        if (game == null)
        {
            return NotFound();
        }

        return Ok(new GameDto(game));
    }
}