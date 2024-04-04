using EzioLearning.Core.Repositories.Auth;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;

namespace EzioLearning.Infrastructure.Repositories.Auth
{
    public class PermissionRepository(EzioLearningDbContext context) :
        RepositoryBase<AppPermission, Guid>(context), IPermissionRepository
    {
        private readonly EzioLearningDbContext _context = context;

        public Task<IEnumerable<AppPermission>> GetByUserId(Guid userId)
        {
            var permissions =
                from user in _context.Users
                from permission in user.Permissions
                where userId.Equals(user.Id)
                select permission;
            return Task.FromResult<IEnumerable<AppPermission>>(permissions);
        }
    }
}
