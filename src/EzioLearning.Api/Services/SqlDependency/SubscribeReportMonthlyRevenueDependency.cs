using EzioLearning.Api.Hubs;
using Microsoft.Data.SqlClient;


namespace EzioLearning.Api.Services.SqlDependency
{
    public class SubscribeReportMonthlyRevenueDependency(ReportHub reportHub, IConfiguration configuration) : ISubscribeTableDependency
    {
        private static Microsoft.Data.SqlClient.SqlDependency? _sqlDependency;
        public async Task SubscribeTableDependency(string connectionString)
        {
            var sqlConnection = new SqlConnection(connectionString);

            var sqlCommand = new SqlCommand("Select CourseId,Id,UserId,Price,Confirm,CreatedDate,ModifiedDate from Learning.Students", sqlConnection);

            _sqlDependency = new Microsoft.Data.SqlClient.SqlDependency(sqlCommand);

            _sqlDependency.OnChange += SqlDependency_OnChange;

            await sqlConnection.OpenAsync();

            await sqlCommand.ExecuteNonQueryAsync();

        }

        private async void SqlDependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            try
            {
                await reportHub.SendMonthlyRevenue();

                if (_sqlDependency != null) _sqlDependency.OnChange -= SqlDependency_OnChange;

                await SubscribeTableDependency(configuration.GetConnectionString(nameof(EzioLearning))!);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

        }
    }
}
