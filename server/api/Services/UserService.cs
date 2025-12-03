using api.Models;
using api.Models.Dtos.Requests.User;
using api.Security;
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

    public async Task<UserDto> CreateUser(CreateUserDto createUserDto)
    {
        // Optional: Check if user with email already exists
        var existingUser = await userRepository.Query()
            .FirstOrDefaultAsync(u => u.Email == createUserDto.Email && u.DeletedAt == null);

        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email {createUserDto.Email} already exists");
        }

        var hasher = new PasswordHasher();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            PhoneNumber = createUserDto.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = createUserDto.ActivateMembership ? DateTime.UtcNow.AddYears(1) : null,
            PasswordHash = hasher.GenerateRandomPassword(new Random().Next(1,64))
        };

        await userRepository.AddAsync(user);

        return new UserDto(user);
    }

    public async Task<UserDto> UpdateUser(Guid id, UpdateUserDto updateUserDto)
    {
        var user = await userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with id {id} not found");
        }

        // Update only the properties that are provided
        if (!string.IsNullOrWhiteSpace(updateUserDto.Name))
        {
            user.Name = updateUserDto.Name;
        }

        if (!string.IsNullOrWhiteSpace(updateUserDto.Email))
        {
            user.Email = updateUserDto.Email;
        }

        // Add other property updates as needed

        await userRepository.UpdateAsync(user);

        return new UserDto(user);
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

    public async Task<UserDto> RenewMembership(Guid id)
    {
        var user = await userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with id {id} not found");
        }

        if (user.ExpiresAt == null)
        {
            throw new InvalidOperationException("User does not have an active membership to renew");
        }

        var now = DateTime.UtcNow;
        var expiresAt = user.ExpiresAt.Value;
        var renewalWindowStart = expiresAt.AddHours(-168);

        if (expiresAt < now)
        {
            user.ExpiresAt = now.AddYears(1);
        }
        else if (now >= renewalWindowStart)
        {
            user.ExpiresAt = expiresAt.AddYears(1);
        }
        else
        {
            // Too early to renew
            var daysUntilRenewal = (renewalWindowStart - now).TotalDays;
            throw new InvalidOperationException(
                $"Membership can only be renewed within 48 hours of expiration. " +
                $"Renewal available in {Math.Ceiling(daysUntilRenewal)} days");
        }

        await userRepository.UpdateAsync(user);

        return new UserDto(user);
    }
}