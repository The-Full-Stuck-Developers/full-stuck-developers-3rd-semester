namespace api.Models.Dtos.Responses;

public record AuthUserInfo(Guid Id, string Email, string PhoneNumber, string Name, bool IsAdmin, int Balance);

public record LoginResponse(string Jwt);

public record RegisterResponse(string Name);