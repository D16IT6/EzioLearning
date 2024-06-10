using AutoMapper;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;
using EzioLearning.Share.Utils;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.Repositories.Learning;

public class CourseRepository(EzioLearningDbContext context, IMapper mapper)
    : PagedRepositoryBase<Course, Guid>(context, mapper), ICourseRepository
{
    public async Task<int> CountCourses()
    {
        return await DbSet.CountAsync(x => x.Status == CourseStatus.Ready);
    }

    public async Task<IEnumerable<Course>> GetFeaturedCourses(int take = 12)
    {
        var data = (await GetAllAsync([nameof(Course.User)])).AsQueryable()
            .Where(x => !x.User!.IsDeleted && x.Status == CourseStatus.Ready)
            .OrderByDescending(x => x.Students.Count())
            .Take(take);

        return data;
    }

     
}