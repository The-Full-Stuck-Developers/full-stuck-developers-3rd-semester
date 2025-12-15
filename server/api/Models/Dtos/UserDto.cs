using dataccess.Entities;
namespace Dtos;

public class UserDto
{
    public UserDto(User entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        // Password = entity.PasswordHash;
        Email = entity.Email;
        PhoneNumber = entity.PhoneNumber;
        IsAdmin = entity.IsAdmin;
        IsActive = entity.IsActive;
        ExpiresAt = entity.ExpiresAt;
        CreatedAt = entity.CreatedAt;
        UpdatedAt = entity.UpdatedAt;
        DeletedAt = entity.DeletedAt;
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool IsAdmin { get; set; }

    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
