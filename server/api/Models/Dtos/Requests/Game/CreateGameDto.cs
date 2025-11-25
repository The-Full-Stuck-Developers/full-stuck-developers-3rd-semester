using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests.Game;

public record CreateGameDto
{
    public DateTime? StartTime { get; set; }

    [Required]
    public bool IsActive { get; set; }

    [Required]
    [RegularExpression(@"^\d,\d,\d$", 
        ErrorMessage = "WinningNumbers must contain exactly 3 numbers separated by commas, e.g. '1,5,9'.")]
    public string WinningNumbers { get; set; }
    
}