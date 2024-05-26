﻿using System.Linq.Expressions;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Core.Repositories.Auth
{
    public interface IPermissionRepository : IRepository<AppPermission, Guid>
    {
        public Task<IEnumerable<AppPermission>> GetByUserId(Guid userId, Expression<Func<AppPermission,bool>>? predicate = null);
    }
}