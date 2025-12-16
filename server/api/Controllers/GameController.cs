using api.Models;
using api.Services;
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
[Authorize]
public class GamesController(
    IGameService gameService)
    : ControllerBase
{
    [HttpGet("GetAllUpcomingGames")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<PagedResult<GameDto>>> GetAllUpcomingGames([FromQuery] SieveModel sieveModel)
    {
        var games = await gameService.GetAllUpcomingGames(sieveModel);

        return Ok(games);
    }

    [HttpGet("GetAllPastGames")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<PagedResult<GameDto>>> GetAllPastGames([FromQuery] SieveModel sieveModel)
    {
        var games = await gameService.GetAllPastGames(sieveModel);

        return Ok(games);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<GameDto>> GetGameById(Guid id)
    {
        var game = await gameService.GetGameById(id);

        return Ok(game);
    }
}
