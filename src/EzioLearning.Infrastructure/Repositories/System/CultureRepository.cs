using EzioLearning.Core.Repositories.System;
using EzioLearning.Domain.Entities.System;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;

namespace EzioLearning.Infrastructure.Repositories.System
{
    public class CultureRepository(EzioLearningDbContext context) : RepositoryBase<Culture, string>(context), ICultureRepository
    {
    }
}
