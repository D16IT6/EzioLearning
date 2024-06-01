using System.Linq.Expressions;
using EzioLearning.Core.Repositories.Auth;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.Repositories.Auth
{
    public class PermissionRepository(EzioLearningDbContext context) :
        RepositoryBase<AppPermission, Guid>(context), IPermissionRepository
    {
        private readonly EzioLearningDbContext _context = context;

        public async Task<IEnumerable<AppPermission>> GetByUserId(Guid userId, Expression<Func<AppPermission, bool>>? predicate = null)
        {
            var permissions =
                await (from user in _context.Users
                from permission in user.Permissions
                where userId.Equals(user.Id) && !user.IsDeleted
                select permission).ToListAsync();

            return permissions;
        }
    }
}
