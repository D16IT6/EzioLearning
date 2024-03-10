using System.Text.Json;
using EzioLearning.Domain.Common;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Authorization;

namespace EzioLearning.Api.Authorization
{
    public class PermissionAuthorizationHandler(IHttpContextAccessor accessor)
        : AuthorizationHandler<PermissionRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
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
                .Where(x => x.Type.Equals(CustomClaimTypes.Permissions)).ToList();
            if (permissionClaims.Count == 0) return Task.CompletedTask;

            var userPermissions = permissionClaims.Select(x => x.Value).ToArray();

            if (userPermissions.Contains(permission))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
            //// Custom response -> Custom Middleware
            //var httpContext = accessor.HttpContext;
            //httpContext!.Response.StatusCode = StatusCodes.Status403Forbidden;
            //httpContext.Response.ContentType = "application/json";

            //var response = new ResponseBase
            //{
            //    Errors = new Dictionary<string, string[]>
            //    {
            //        { "UnAuthorize", ["Bạn không có quyền truy cập chức năng này"] }
            //    }
            //};
            //var jsonResponse = JsonSerializer.Serialize(response);
            //return httpContext.Response.WriteAsync(jsonResponse);

        }
    }
}
