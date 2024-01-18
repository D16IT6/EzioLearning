﻿

using EzioLearning.Core.SeedWorks;
using EzioLearning.Infrastructure.DbContext;

namespace EzioLearning.Infrastructure.SeedWorks
{
    public class UnitOfWork(EzioLearningDbContext context) : IUnitOfWork
    {
        public async Task<int> CompleleAsync()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
