using EzioLearning.Core.Repositories;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Api.Services
{
    public class PermissionService(UserManager<AppUser> userManager, IPermissionRepository permissionRepository)
    {
        private readonly List<AppPermission> _permissions = permissionRepository.GetAllAsync().Result.ToList();

        public async Task AddPermissionsForNewUser(AppUser user)
        {
            AddPermissionWithName(user, "Permissions.Accounts");

            if (await userManager.IsInRoleAsync(user, RoleConstants.Teacher))
            {
                AddPermissionWithName(user, "Permissions.Courses");
            }

            await userManager.UpdateAsync(user);
        }

        private void AddPermissionWithName(AppUser user, string prefix)
        {
            var userPermission = _permissions.Where(x => x.Name.StartsWith(prefix));
            foreach (var permission in userPermission)
            {
                user.Permissions.Add(permission);
            }
        }
    }
}
