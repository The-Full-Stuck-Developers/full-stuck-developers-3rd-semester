using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests.User;

public record CreateUserDto
{
    [Required(ErrorMessage = "required")]
    [MinLength(3, ErrorMessage = "min_length_3")]
    public string Name { get; set; }

    [Required(ErrorMessage = "required")]
    [EmailAddress(ErrorMessage = "email_address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "required")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "phone_regex_min_8")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "required")] public bool IsAdmin { get; set; }

    [Required(ErrorMessage = "required")] public bool ActivateMembership { get; set; }
}