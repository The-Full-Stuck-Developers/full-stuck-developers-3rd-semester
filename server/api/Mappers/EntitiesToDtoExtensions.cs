using System.ComponentModel.DataAnnotations;
using api.Models.Dtos.Responses;
using dataccess.Entities;

namespace api.Mappers;

public static class EntitiesToDtoExtensions
{
    public static AuthUserInfo ToDto(this User user)
    {
        return new AuthUserInfo(
            Id: user.Id,
            Name: user.Name,
            IsAdmin: user.IsAdmin,
            ExpiresAt: user.ExpiresAt,
            DeletedAt: user.DeletedAt
        );
    }

}
