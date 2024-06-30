using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Share.Dto.Learning.Course;

namespace EzioLearning.Core.Repositories.Learning
{
    public interface IStudentRepository : IRepository<Student,Guid>
    {
        Task<IEnumerable<MonthlyRevenueItem>> GetMonthlyIncome(int year, Guid teacherId);
    }
}
