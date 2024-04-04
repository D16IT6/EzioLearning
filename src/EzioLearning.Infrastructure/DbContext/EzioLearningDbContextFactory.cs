using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EzioLearning.Infrastructure.DbContext
{
    internal class EzioLearningDbContextFactory: IDesignTimeDbContextFactory<EzioLearningDbContext>
    {
        public EzioLearningDbContext CreateDbContext(string[] args)
        {

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var connectionString = configuration.GetConnectionString(nameof(EzioLearning));

            var optionsBuilder = new DbContextOptionsBuilder<EzioLearningDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new EzioLearningDbContext(optionsBuilder.Options);

        }
    }
}
