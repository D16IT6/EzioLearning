using System.Data;
using System.Security.Claims;
using Dapper;
using EzioLearning.Share.Common;
using EzioLearning.Share.Dto.Learning.Course;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;

namespace EzioLearning.Api.Hubs
{
    [Authorize(Permissions.Dashboard.View)]
    public class ReportHub(IConfiguration configuration) : Hub
    {
        public async Task SendMonthlyRevenue()
        {
            try
            {
                if (Context.User is { Identity.IsAuthenticated: true })
                {
                    _ = Guid.TryParse(Context.User?.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value,
                        out var teacherId);

                    if (teacherId != Guid.Empty)
                    {
                        var data = await FetchData(teacherId);
                        await Clients.All.SendAsync("ReceiveMonthlyRevenue", data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task<IEnumerable<MonthlyRevenueItem>> FetchData(Guid teacherId)
        {
            var sqlConnection = new SqlConnection(configuration.GetConnectionString(nameof(EzioLearning)));

            return await sqlConnection.QueryAsync<MonthlyRevenueItem>("usp_ReportMonthlyRevenue",new
            {
                teacherId
            },commandType:CommandType.StoredProcedure);
        }
    }
}
