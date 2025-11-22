using System.ComponentModel.DataAnnotations;

namespace api.Models;

public class AppOptions
{
    [Required] [MinLength(1)] public string DefaultConnection { get; set; } = null!;
    [Required] [MinLength(1)] public string JwtSecret { get; set; }
}