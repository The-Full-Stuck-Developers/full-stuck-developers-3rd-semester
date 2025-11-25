using api.Models.Dtos.Requests.User;
using Dtos;

namespace api.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers();
    Task<UserDto?> GetUserById(Guid id);
    Task<UserDto> CreateUser(CreateUserDto dto);
    Task<UserDto> UpdateUser(UpdateUserDto dto);
    Task DeleteUser(Guid id);
}