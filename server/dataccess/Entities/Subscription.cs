using System.ComponentModel.DataAnnotations;
using DefaultNamespace;

namespace dataccess.Entities;

public class Subscription
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid CreatedByAdminId { get; set; }   
    public User CreatedByAdmin { get; set; } = null!;

    public DateTime ValidFrom { get; set; }      
    public DateTime ValidTo { get; set; }       

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public Guid? RevokedByAdminId { get; set; }

    public bool IsActiveAt(DateTime date) => 
        RevokedAt == null && date >= ValidFrom && date <= ValidTo;
}