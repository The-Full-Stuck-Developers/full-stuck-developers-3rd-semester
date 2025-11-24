using api.Models;
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
public class UsersController(
    IRepository<User> userRepository,
    ISieveProcessor sieveProcessor)
    : ControllerBase
{

    [HttpGet]
    // [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAllUsers([FromQuery] SieveModel sieveModel)
    {
        Console.Write("tuka smeee");

        var query = userRepository.Query();
        var result = sieveProcessor.Apply(sieveModel, query);
        var total = await userRepository.Query().CountAsync();
        var items = await result.ToListAsync();
        var users = items.Select(u => new UserDto(u)).ToList();

        return Ok(new PagedResult<UserDto>
        {
            Items = users,
            Total = total,
            PageSize = sieveModel.PageSize ?? 10,
            PageNumber = (sieveModel.Page ?? 1)
        });
    }

    [HttpGet("{id}")]
    // [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<UserDto>> GetUserById(string id)
    {
        var user = await userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(new UserDto(user));
    }
}