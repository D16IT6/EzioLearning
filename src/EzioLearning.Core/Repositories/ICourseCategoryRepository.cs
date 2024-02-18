using EzioLearning.Core.Dtos.Learning.CourseCategory;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;

namespace EzioLearning.Core.Repositories
{
    public interface ICourseCategoryRepository : IPagedRepository<CourseCategory, Guid,CourseCategoryViewDto>
    {
    }
}
