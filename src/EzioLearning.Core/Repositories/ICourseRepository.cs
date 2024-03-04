using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;

namespace EzioLearning.Core.Repositories;

public interface ICourseRepository : IPagedRepository<Course, Guid>
{
    Task<int> CountCourses();
}