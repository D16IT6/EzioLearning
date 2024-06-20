using EzioLearning.Core.Repositories.Resources;
using EzioLearning.Domain.Entities.Resources;
using EzioLearning.Infrastructure.SeedWorks;
using EzioLearning.Infrastructure.DbContext;

namespace EzioLearning.Infrastructure.Repositories.Resources
{
    public class DocumentRepository(EzioLearningDbContext context) : RepositoryBase<Document, Guid>(context), IDocumentRepository
    {
    }
}
