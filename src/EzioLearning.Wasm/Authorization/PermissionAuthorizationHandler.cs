using EzioLearning.Share.Common;
using EzioLearning.Share.Utils;
using Microsoft.AspNetCore.Authorization;

namespace EzioLearning.Wasm.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity == null) return Task.CompletedTask;

        if (!context.User.Identity.IsAuthenticated) return Task.CompletedTask;

        if (context.User.IsInRole(RoleConstants.Admin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var permission = requirement.Permission;

        var permissionClaims = context.User.Claims
            .Where(x => x.Type.Equals(CustomClaimTypes.Permissions)).Select(x => x.Value).ToList();


        var userHasPermission = permissionClaims.Contains(permission);

        if (!userHasPermission) return Task.CompletedTask;

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}