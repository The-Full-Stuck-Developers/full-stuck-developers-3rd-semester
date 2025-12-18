using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests.Game;

public record InPersonDto
{
    [Required]
    public int InPersonWinners { get; set; }

    [Required]
    public int InPersonPrizePool { get; set; }
}
