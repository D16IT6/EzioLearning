using AutoMapper;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;

namespace EzioLearning.Infrastructure.Repositories.Learning;

public class CourseCategoryRepository(EzioLearningDbContext context, IMapper mapper)
    : PagedRepositoryBase<CourseCategory, Guid>(context, mapper),
        ICourseCategoryRepository
{
}