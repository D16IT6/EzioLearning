using System.Net.Http.Json;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Services.Interface;

namespace EzioLearning.Wasm.Services.Implement
{
    public class ReportService(HttpClient httpClient) : IReportService
    {
        public async Task<ResponseBaseWithList<MonthlyRevenueItem>> GetCourseBetSellerReport(int year)
        {
            var response =
                await httpClient.GetFromJsonAsync<ResponseBaseWithList<MonthlyRevenueItem>>($"api/Report/MonthlyIncome?year={year}");

            return response!;
        }
    }
}
