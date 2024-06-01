using AutoMapper;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Utils;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.Repositories.Learning;

public class CourseRepository(EzioLearningDbContext context, IMapper mapper)
    : PagedRepositoryBase<Course, Guid>(context, mapper), ICourseRepository
{
    private readonly IMapper _mapper = mapper;

    public async Task<int> CountCourses()
    {
        return await DbSet.CountAsync(x => x.Status == CourseStatus.Ready);
    }

    public async Task<IEnumerable<Course>> GetFeaturedCourses(int take = 12)
    {
        var data = (await GetAllAsync([nameof(Course.User)]))
            .Where(x => !x.User!.IsDeleted && x.Status == CourseStatus.Ready)
            .OrderByDescending(x => x.Students.Count)
            .ThenByDescending(x => x.CreatedDate)
            .Take(take);

      
        return data;
    }
}