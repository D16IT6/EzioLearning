using AutoMapper;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;
using EzioLearning.Share.Utils;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.Repositories.Learning;

public class CourseRepository(
    EzioLearningDbContext context, 
    ICourseSectionRepository courseSectionRepository,
    ICourseLectureRepository courseLectureRepository,
    IMapper mapper
    ) : PagedRepositoryBase<Course, Guid>(context, mapper), ICourseRepository
{
    public async Task<int> CountCourses()
    {
        return await DbSet.CountAsync(x => x.Status == CourseStatus.Ready);
    }

    public async Task<IEnumerable<Course>> GetFeaturedCourses(int take = 12)
    {
        var data = (await GetAllAsync([nameof(Course.User)])).AsQueryable()
            .Where(x=> x.Status == CourseStatus.Ready)
            .OrderByDescending(x => x.Students.Count())
            .Take(take);

        return data;
    }

    public async Task<int> AddNewSection(Guid courseId, CourseSection courseSection)
    {
        var course = await DbSet.FirstAsync(x => x.Id == courseId);

        var courseSectionNames = course.Sections.Select(x => x.Name);
        if (courseSectionNames.Contains(courseSection.Name)) return -1;

        
        courseLectureRepository.AddRange(courseSection.CourseLectures);

        courseSectionRepository.Add(courseSection);

        course.Sections.Add(courseSection);

        return 1;

    }
}