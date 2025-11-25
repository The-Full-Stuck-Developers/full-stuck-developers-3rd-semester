using System.Security.Claims;
using api.Models.Dtos.Responses;

namespace api.Security;

public static class ClaimExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal claims)
    {
        var value = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }

    public static IEnumerable<Claim> ToClaims(this AuthUserInfo user) =>
        [new("sub", user.Id.ToString()), new("role", user.IsAdmin ? "admin" : "user")];

    public static ClaimsPrincipal ToPrincipal(this AuthUserInfo user) =>
        new ClaimsPrincipal(new ClaimsIdentity(user.ToClaims()));
}