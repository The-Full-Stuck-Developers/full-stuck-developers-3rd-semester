using System.Security.Claims;
using api.Models.Dtos.Responses;

namespace api.Security;

public static class ClaimExtensions
{
    public static string GetUserId(this ClaimsPrincipal claims) =>
        claims.FindFirst(ClaimTypes.NameIdentifier)!.Value;

    public static IEnumerable<Claim> ToClaims(this AuthUserInfo user) =>
        [new("sub", user.Id.ToString()), new("role", user.IsAdmin ? "admin" : "user")];

    public static ClaimsPrincipal ToPrincipal(this AuthUserInfo user) =>
        new ClaimsPrincipal(new ClaimsIdentity(user.ToClaims()));
}