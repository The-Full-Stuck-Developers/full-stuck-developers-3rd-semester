using System.ComponentModel.DataAnnotations;
using dataccess;

namespace api.Models.Dtos.Requests.Transaction;

public record UpdateTransactionDto
{
    [Required]
    public TransactionStatus Status { get; set; }
}