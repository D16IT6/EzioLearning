using System.Security.Claims;
using EzioLearning.Core.Repositories.Auth;
using EzioLearning.Share.Utils;
using Microsoft.AspNetCore.Authorization;

namespace EzioLearning.Api.Authorization;

public class PermissionAuthorizationHandler(IPermissionRepository permissionRepository)
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

        //var permissionClaims = context.User.Claims
        //    .Where(x => x.Type.Equals(CustomClaimTypes.Permissions)).ToList();

        var permissionClaim = permissionRepository.GetByUserId(
            Guid.Parse(context.User.Claims.First(x => x.Type.Equals(ClaimTypes.Sid)).Value)).Result.
                FirstOrDefault(x => x.Name.Equals(permission));
        if (permissionClaim == null) return Task.CompletedTask;

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}