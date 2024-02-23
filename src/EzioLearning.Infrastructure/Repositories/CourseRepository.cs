
using AutoMapper;
using EzioLearning.Core.Repositories;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.Repositories
{
    public class CourseRepository(EzioLearningDbContext context, IMapper mapper) : PagedRepositoryBase<Course, Guid>(context, mapper), ICourseRepository
    {
        public async Task<int> CountCourses()
        {
            return await DbSet.CountAsync(x => x.Status == CourseStatus.Ready);
        }
    }
}
