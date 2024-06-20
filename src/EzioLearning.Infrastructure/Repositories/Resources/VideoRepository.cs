using EzioLearning.Core.Repositories.Resources;
using EzioLearning.Domain.Entities.Resources;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;

namespace EzioLearning.Infrastructure.Repositories.Resources
{
    public class VideoRepository(EzioLearningDbContext context):RepositoryBase<Video,Guid>(context),IVideoRepository
    {
    }
}
