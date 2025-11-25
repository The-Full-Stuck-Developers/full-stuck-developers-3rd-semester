using api.Models;
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
}