using api.Models;
using api.Models.Dtos.Requests.Game;
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

    [HttpGet("GetCurrentGame")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<GameDto>> GetCurrentGame()
    {
        var game = await gameService.GetCurrentGame();

        return Ok(game);
    }

    [HttpGet("player/current")]
    [Authorize]
    public async Task<ActionResult<GameDto>> GetCurrentGameForPlayer()
    {
        try
        {
            var game = await gameService.GetCurrentGame();
            return Ok(game);
        }
        catch (KeyNotFoundException)
        {
            // No current game exists yet
            return NotFound();
        }
    }

    [HttpPatch("{id:guid}/UpdateWinningNumbers")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<GameDto>> UpdateWinningNumbers(Guid id,
        [FromBody] WinningNumbersDto winningNumbersDto)
    {
        var game = await gameService.UpdateWinningNumbers(id, winningNumbersDto);

        return Ok(game);
    }

    [HttpPatch("{id:guid}/DrawWinners")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<GameDto>> DrawWinners(Guid id)
    {
        var game = await gameService.DrawWinners(id);

        return Ok(game);
    }

    [HttpPatch("{id:guid}/UpdateInPersonData")]
    [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<GameDto>> UpdateInPersonData(Guid id,
        [FromBody] InPersonDto dto)
    {
        var game = await gameService.UpdateInPersonData(id, dto);
        return Ok(game);
    }
}
