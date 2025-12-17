using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DefaultNamespace;

namespace dataccess.Entities;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; }

    [JsonIgnore]
    public string PasswordHash { get; set; }
    
    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public bool IsAdmin { get; set; }

    [Column("expires_at")]
    public DateTime? ExpiresAt { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [Column("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }
}
