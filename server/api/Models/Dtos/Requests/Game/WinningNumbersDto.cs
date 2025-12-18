using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests.Game;

public record WinningNumbersDto
{
    [Required]
    [RegularExpression(@"^\d{1,2},\d{1,2},\d{1,2}$",
        ErrorMessage = "WinningNumbers must contain exactly 3 numbers separated by commas, e.g. '1,5,9'.")]
    public string? WinningNumbers { get; set; }
}
