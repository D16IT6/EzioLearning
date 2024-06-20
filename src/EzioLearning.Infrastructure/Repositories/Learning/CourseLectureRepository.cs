using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;

namespace EzioLearning.Infrastructure.Repositories.Learning
{
    public class CourseLectureRepository(EzioLearningDbContext context)
        : RepositoryBase<CourseLecture, Guid>(context), ICourseLectureRepository
    {
    }
}
