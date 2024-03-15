﻿using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Identity;

namespace EzioLearning.Core.Repositories
{
    public interface IPermissionRepository : IRepository<AppPermission, Guid>
    {
        public Task<IEnumerable<AppPermission>> GetByUserId(Guid userId);
    }
}