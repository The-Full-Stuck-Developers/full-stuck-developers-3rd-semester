using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests.User;

public record CreateUserDto
{
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MinLength(3)]
        public string Email { get; set; }

        [Required]
        [MinLength(4)]
        public string PhoneNumber { get; set; }

        [Required]
        public bool IsAdmin { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public DateTime? ExpiresAt { get; set; }
}