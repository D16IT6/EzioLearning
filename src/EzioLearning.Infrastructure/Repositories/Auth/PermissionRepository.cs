using System.Linq.Expressions;
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

        public Task<IEnumerable<AppPermission>> GetByUserId(Guid userId, Expression<Func<AppPermission, bool>>? predicate = null)
        {


            var permissions = from p in _context.Permissions
                              from u in _context.Users
                              where p.Id.Equals(u.Id)
                              && u.Id == userId && !u.IsDeleted
                              select p;

            if (predicate != null)
            {
                permissions = permissions.Where(predicate);
            }

            return Task.FromResult<IEnumerable<AppPermission>>(permissions);
        }
    }
}
