using EzioLearning.Api.Hubs;
using EzioLearning.Domain.Entities.Learning;
using TableDependency.SqlClient;


namespace EzioLearning.Api.Services.SqlDependency
{
    public class SubscribeReportMonthlyRevenueDependency(ReportHub reportHub, IConfiguration configuration) : ISubscribeTableDependency
    {
        private static SqlTableDependency<Student>? _sqlDependency;
        public Task SubscribeTableDependency(string connectionString)
        {
            _sqlDependency = new SqlTableDependency<Student>(connectionString, $"{nameof(Student)}s", "Learning");

            _sqlDependency.OnChanged += _sqlDependency_OnChanged; ;

            _sqlDependency.Start();
            return Task.CompletedTask;
        }

        private async void _sqlDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Student> e)
        {
            try
            {
                await reportHub.SendMonthlyRevenue();

                if (_sqlDependency == null) return;

                _sqlDependency.Stop();
                await SubscribeTableDependency(configuration.GetConnectionString(nameof(EzioLearning))!);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
