using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;

namespace EzioLearning.Core.Repositories.Learning;

public interface ICourseRepository : IPagedRepository<Course, Guid>
{
    Task<int> CountCourses();
    Task<IEnumerable<Course>> GetFeaturedCourses(int take = 12);
    Task<int> AddNewSection(Guid courseId, CourseSection newSection);
}