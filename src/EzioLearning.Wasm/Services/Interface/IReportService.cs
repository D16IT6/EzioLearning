using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Models.Response;

namespace EzioLearning.Wasm.Services.Interface
{
    public interface IReportService : IServiceBase
    {
        Task<ResponseBaseWithList<MonthlyRevenueItem>> GetCourseBetSellerReport(int year);
    }
}
