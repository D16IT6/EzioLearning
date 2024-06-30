namespace EzioLearning.Api.Services.SqlDependency
{
    public interface ISubscribeTableDependency
    {
        Task SubscribeTableDependency(string connectionString);
    }
}
