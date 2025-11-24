using Microsoft.AspNetCore.Authorization;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace api.Security;

public class IsAdmin : IAuthorizationRequirement
{
}

public class AdminAuthorizationHandler(IRepository<User> userRepository) : AuthorizationHandler<IsAdmin>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IsAdmin requirement)
    {
        var userIdClaim = context.User.FindFirst("sub")?.Value
                          ?? context.User.FindFirst("id")?.Value;

        var userId = Guid.Parse(userIdClaim!);

        if (string.IsNullOrEmpty(userIdClaim))
        {
            context.Fail();
            return;
        }

        var user = await userRepository.Query()
            .FirstOrDefaultAsync(u => u.Id == userId);

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