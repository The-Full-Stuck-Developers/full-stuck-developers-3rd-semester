using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace api.Security;

public class IsAdmin : IAuthorizationRequirement
{
}

public class AdminAuthorizationHandler
    : AuthorizationHandler<IsAdmin>
{
    private readonly IRepository<User> _userRepository;

    public AdminAuthorizationHandler(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IsAdmin requirement)
    {
        // Ensure the user is authenticated
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Fail();
            return;
        }

        // Fast path: check for an "is_admin" claim
        var isAdminClaim = context.User.FindFirst("is_admin")?.Value;
        if (bool.TryParse(isAdminClaim, out var isAdminFromClaim))
        {
            if (isAdminFromClaim)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return;
        }

        // Fallback: resolve user ID safely
        var userIdClaim =
            context.User.FindFirst("sub")?.Value
            ?? context.User.FindFirst("id")?.Value
            ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return; // 403 will result
        }

        // Database lookup (only if claim not present)
        var user = await _userRepository.Query()
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.IsAdmin })
            .FirstOrDefaultAsync();

        if (user?.IsAdmin == true)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
