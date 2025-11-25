using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests.Game;

public record UpdateGameDto
{
    public DateTime? StartTime { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    [RegularExpression(@"^\d{3}$",
        ErrorMessage = "WinningNumbers must contain exactly 3 digits (e.g. '159').")]
    public string WinningNumbers { get; set; }
}