namespace dataccess;

public class User
{
    public string Id { get; set; }

    public string Name { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public bool IsAdmin { get; set; }

    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }
}