using AutoMapper;
using EzioLearning.Core.Repositories;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;

namespace EzioLearning.Infrastructure.Repositories;

public class CourseCategoryRepository(EzioLearningDbContext context, IMapper mapper)
    : PagedRepositoryBase<CourseCategory, Guid>(context, mapper),
        ICourseCategoryRepository
{
}