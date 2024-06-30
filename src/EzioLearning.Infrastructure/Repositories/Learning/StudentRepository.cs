using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;
using EzioLearning.Share.Dto.Learning.Course;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.Repositories.Learning
{
    public class StudentRepository(EzioLearningDbContext context) : RepositoryBase<Student, Guid>(context), IStudentRepository
    {
        public Task<IEnumerable<MonthlyRevenueItem>> GetMonthlyIncome(int year, Guid teacherId)
        {
            var students = DbSet.AsQueryable();

            students = students.Include(x => x.Course);

            var data = from student in students
                       where teacherId == student.Course.CreatedBy
                       && student.Confirm
                       && student.ModifiedDate.Value.Year == year
                       select student;

            var monthlyRevenue = data.GroupBy(x => x.CreatedDate.Month)
                .Select(g => new MonthlyRevenueItem
                {
                    Month = g.Key,
                    TotalPrice = g.Sum(x => x.Price)
                });
            return Task.FromResult<IEnumerable<MonthlyRevenueItem>>(monthlyRevenue);

        }
    }
}
