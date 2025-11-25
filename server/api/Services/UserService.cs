using System.ComponentModel.DataAnnotations;
using api.Models.Dtos.Requests.User;
using dataccess;
using dataccess.Entities;
using Dtos;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class UserService(MyDbContext dbContext) : IUserService
{
    public Task<List<UserDto>> GetAllUsers()
    {
        return dbContext.Users.Select(u => new UserDto(u)).ToListAsync();
    }

    public Task<UserDto?> GetUserById(Guid id)
    {
        return dbContext.Users.Where(u => u.Id == id).Select(u => new UserDto(u)).FirstOrDefaultAsync();
    }

    public async Task<UserDto> CreateUser(CreateUserDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            IsAdmin = false,
        };
            
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return new UserDto(user);
    }

    public async Task<UserDto> UpdateUser(UpdateUserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUser(Guid id)
    {
        throw new NotImplementedException();
    }
}