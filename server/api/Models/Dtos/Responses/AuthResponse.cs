namespace api.Models.Dtos.Responses;

public record AuthUserInfo(string Id, string Name, bool IsAdmin);

public record LoginResponse();

public record RegisterResponse(string Name);