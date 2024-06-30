using AutoMapper;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Core.Repositories.Resources;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Utils;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.Repositories.Learning;

public class CourseRepository(
    EzioLearningDbContext context,
    ICourseSectionRepository courseSectionRepository,
    ICourseLectureRepository courseLectureRepository,
    IVideoRepository videoRepository,
    IDocumentRepository documentRepository,
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
            .Where(x => x.Status == CourseStatus.Ready)
            .OrderByDescending(x => x.Students.Count())
            .Take(take);

        return data;
    }

    public async Task<int> AddNewSection(Guid courseId, CourseSection courseSection)
    {
        var course = await DbSet.FirstAsync(x => x.Id == courseId);

        var courseSectionNames = course.Sections.Select(x => x.Name);
        if (courseSectionNames.Contains(courseSection.Name)) return -1;

        courseSectionRepository.Add(courseSection);
        course.Sections.Add(courseSection);

        foreach (var lecture in courseSection.CourseLectures)
        {
            courseLectureRepository.Add(lecture);
            switch (lecture)
            {
                case { LectureType: CourseLectureType.Video, Video: not null }:
                    videoRepository.Add(lecture.Video);
                    break;
                case { Document: not null, LectureType: CourseLectureType.Document }:
                    documentRepository.Add(lecture.Document);
                    break;
            }
        }
        return 1;

    }

    public async Task<Course?> GetCourseDetail(Guid courseId)
    {


        var queryable = DbSet.AsQueryable();

        queryable = queryable
            .Include(x => x.Sections)
            .ThenInclude(s => s.CourseLectures)
            .ThenInclude(l => l.Video)

            .Include(x => x.Sections)
            .ThenInclude(s => s.CourseLectures)
            .ThenInclude(l => l.Document)

            .Include(c => c.User)
            .AsSplitQuery()
            ;



        var course = await queryable
            .FirstOrDefaultAsync(x => x.Id == courseId);
        return course;
    }

    public async Task<Course?> GetCourseUpdate(Guid courseId, Guid teacherId)
    {
        var queryable = DbSet.AsQueryable();

        queryable = queryable
                .Include(x => x.Categories)
                .ThenInclude(x => x.CourseCategoryTranslations)
                .ThenInclude(x => x.Culture)

                .Include(x => x.Sections)
                .ThenInclude(s => s.CourseLectures)
                .ThenInclude(l => l.Video)

                .Include(x => x.Sections)
                .ThenInclude(s => s.CourseLectures)
                .ThenInclude(l => l.Document)

                .AsSplitQuery()
            ;


        var course = await queryable
            .FirstOrDefaultAsync(x => x.Id == courseId && x.CreatedBy == teacherId);
        return course;
    }

    public async Task<Course?> FindCourseUpdate(Guid courseId)
    {
        var queryable = DbSet.AsQueryable();

        queryable = queryable
                .Include(x => x.Categories)
                //.ThenInclude(x => x.CourseCategoryTranslations)
                //.ThenInclude(x => x.Culture)

                .Include(x => x.Sections)
                .ThenInclude(s => s.CourseLectures)
                .ThenInclude(l => l.Video)

                .Include(x => x.Sections)
                .ThenInclude(s => s.CourseLectures)
                .ThenInclude(l => l.Document)

                .AsSplitQuery()
        ;
        var course = await queryable
            .FirstOrDefaultAsync(x => x.Id == courseId);
        return course;
    }

}