using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests;

public class RegisterRequestDto
{
    [EmailAddress][Required] public string Email { get; set; } = null!;
    [MinLength(8)] public string Password { get; set; } = null!;
    [Required] public string Name { get; set; } = null!;
}