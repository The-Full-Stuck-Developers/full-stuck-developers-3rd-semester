namespace api.Models.Dtos.Responses;

public record AuthUserInfo(Guid Id, string Name, bool IsAdmin);

public record LoginResponse(string Jwt);

public record RegisterResponse(string Name);
