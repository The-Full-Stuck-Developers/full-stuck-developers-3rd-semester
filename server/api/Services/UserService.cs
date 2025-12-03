using api.Models;
using Dtos;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;
using Sieve.Services;

namespace api.Services;

// public interface IUserService
// {
//     Task<PagedResult<UserDto>> GetAllUsers(SieveModel sieveModel);
//     Task<UserDto?> GetUserById(Guid id);
//     Task DeleteUser(Guid id);
// }

public class UserService(
    IRepository<User> userRepository,
    ISieveProcessor sieveProcessor) : IUserService
{
    public async Task<PagedResult<UserDto>> GetAllUsers(SieveModel sieveModel)
    {
        var query = userRepository.Query().Where(u => u.DeletedAt == null);
        var filteredQuery = sieveProcessor.Apply(sieveModel, query, applyPagination: false);
        var total = await filteredQuery.CountAsync();
        var result = sieveProcessor.Apply(sieveModel, query);
        var items = await result.ToListAsync();
        var users = items.Select(u => new UserDto(u)).ToList();

        return new PagedResult<UserDto>
        {
            Items = users,
            Total = total,
            PageSize = sieveModel.PageSize ?? 10,
            PageNumber = sieveModel.Page ?? 1
        };
    }

    public async Task<UserDto?> GetUserById(Guid id)
    {
        var user = await userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == id);

        return user == null ? null : new UserDto(user);
    }

    public async Task DeleteUser(Guid id)
    {
        var user = await userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with id {id} not found");
        }

        await userRepository.DeleteAsync(user);
    }
}