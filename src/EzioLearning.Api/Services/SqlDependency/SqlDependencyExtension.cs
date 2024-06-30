namespace EzioLearning.Api.Services.SqlDependency
{
    public static class SqlDependencyExtension
    {
        public static void UseSqlTableDependency<T>(this WebApplication app)
            where T : ISubscribeTableDependency
        {
            var serviceProvider = app.Services;
            var service = serviceProvider.GetRequiredService<T>();

            var connectionString = app.Configuration.GetConnectionString(nameof(EzioLearning));
            service.SubscribeTableDependency(connectionString!);
        }
    }
}
