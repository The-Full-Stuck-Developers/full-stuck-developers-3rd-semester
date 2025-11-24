using System.ComponentModel.DataAnnotations;

namespace api.Models.Dtos.Requests.Transaction;

public record CreateTransactionDto
{
    [Required]
    [MinLength(1)]
    public string UserId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int MobilePayTransactionNumber { get; set; }
}