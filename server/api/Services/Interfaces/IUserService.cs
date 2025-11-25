using api.Models;
using api.Models.Dtos.Requests.User;
using Dtos;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace api.Services;

public interface IUserService
{
    Task<PagedResult<UserDto>> GetAllUsers(SieveModel sieveModel);
    Task<UserDto?> GetUserById(Guid id);
    Task DeleteUser(Guid id);
}