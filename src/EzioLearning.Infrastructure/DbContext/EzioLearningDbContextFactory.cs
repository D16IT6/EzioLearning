using EzioLearning.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EzioLearning.Infrastructure.DbContext
{
    public class EzioLearningDbContextFactory : IDesignTimeDbContextFactory<EzioLearningDbContext>
    {
        public EzioLearningDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<EzioLearningDbContext>();

            var connectionString = configuration.GetConnectionString(ConnectionConstants.ConnectionStringName);


            builder.UseSqlServer(connectionString);

            return new EzioLearningDbContext(builder.Options);
        }
    }
}
