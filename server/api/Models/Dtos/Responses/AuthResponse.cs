namespace api.Models.Dtos.Responses;

public record AuthUserInfo(Guid Id, string Name, bool IsAdmin, DateTime? ExpiresAt, DateTime? DeletedAt);

public record LoginResponse(string Jwt);

public record RegisterResponse(string Name);
