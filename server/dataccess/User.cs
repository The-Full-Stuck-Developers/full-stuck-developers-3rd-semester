namespace dataccess;

public class User
{
    public string Id { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime DeletedAt { get; set; }

    public string Role { get; set; } = null!;
}