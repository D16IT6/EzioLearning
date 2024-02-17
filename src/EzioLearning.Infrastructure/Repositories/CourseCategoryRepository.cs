using EzioLearning.Core.Repositories;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;

namespace EzioLearning.Infrastructure.Repositories
{
    public class CourseCategoryRepository(EzioLearningDbContext context) 
        : RepositoryBase<CourseCategory, Guid>(context),ICourseCategoryRepository
    {
        
    }
}
