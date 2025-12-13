using api.Models;
using api.Models.Dtos.Requests.User;
using api.Services;
using Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
// [Authorize]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    // [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<PagedResult<UserDto>>> GetAllUsers([FromQuery] SieveModel sieveModel)
    {
        var result = await userService.GetAllUsers(sieveModel);
        return Ok(result);
    }

    [HttpGet("{id}")]
    // [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await userService.GetUserById(id);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost]
// [Authorize(Policy = "IsAdmin")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        try
        {
            var createdUser = await userService.CreateUser(createUserDto);
            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdUser.Id },
                createdUser
            );
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}")]
// [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<UserDto>> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var updatedUser = await userService.UpdateUser(id, updateUserDto);
            return Ok(updatedUser);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    // [Authorize(Policy = "IsAdmin")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        try
        {
            await userService.DeleteUser(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{id}/renew-membership")]
// [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<UserDto>> RenewMembership(Guid id)
    {
        try
        {
            var renewedUser = await userService.RenewMembership(id);
            return Ok(renewedUser);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"User with id {id} not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("GetPlayerCount")]
// [Authorize(Policy = "IsAdmin")]
    public async Task<ActionResult<int>> GetPlayersCount()
    {
        var count = await userService.GetUsersCount();

        return Ok(new { count = count });
    }
}
