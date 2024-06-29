using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;

namespace EzioLearning.Infrastructure.Repositories.Learning
{
    public class StudentRepository(EzioLearningDbContext context) : RepositoryBase<Student,Guid>(context), IStudentRepository
    {

    }
}
