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
    Task<UserDto> CreateUser(CreateUserDto createUserDto);
    Task<UserDto> UpdateUser(Guid id, UpdateUserDto updateUserDto);
    Task DeleteUser(Guid id);
    Task<UserDto> RenewMembership(Guid id);
    Task<int> GetUsersCount();
    Task<UserDto> ActivateUser(Guid id);
    Task<UserDto> DeactivateUser(Guid id);
}
