using System.ComponentModel.DataAnnotations;
using api.Models.Dtos.Responses;
using dataccess.Entities;

namespace api.Mappers;

public static class EntitiesToDtoExtensions
{
    public static AuthUserInfo ToDto(this User user)
    {
        return new AuthUserInfo(Id: user.Id, Email: user.Email, PhoneNumber: user.PhoneNumber, Name: user.Name, IsAdmin: user.IsAdmin, user.Balance);
    }
}