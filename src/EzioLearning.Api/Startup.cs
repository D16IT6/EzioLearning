using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.DTO;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Api
{
    public static class Startup
    {
        public static void ConfigureBuilder(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var configuration = builder.Configuration;

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddCors(option =>
            {
                option.AddPolicy("CorsPolicy", policyBuilder =>
                {
                    policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            services.AddDbContext<EzioLearningDbContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString(ConnectionConstants.ConnectionStringName));
            });


            services.ConfigureIndentity();

            services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            var repositoryServices = typeof(RepositoryBase<,>).Assembly.GetTypes()
                .Where(x =>
                    x.GetInterfaces().Any(i => i.Name == typeof(IRepository<,>).Name)
                    && x is { IsAbstract: false, IsClass: true, IsGenericType: false }
                );

            foreach (var repositoryService in repositoryServices)
            {
                var allInterfaces = repositoryService.GetInterfaces();
                var directInterface = allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).FirstOrDefault();
                if (directInterface != null)
                {
                    services.Add(new ServiceDescriptor(directInterface, repositoryService, ServiceLifetime.Scoped));
                }
            }

            services.AddAutoMapper(typeof(UserDto));
        }

        public static void Configure(this WebApplication app)
        {

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                ConnectionConstants.ConnectionStringName = ConnectionStringName.Development;
            }

            ConnectionConstants.ConnectionStringName = ConnectionStringName.Production;

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.MapControllers();

            app.MigrateData();

        }

        public static void ConfigureIndentity(this IServiceCollection service)
        {
            service.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<EzioLearningDbContext>();

            service.Configure<IdentityOptions>(option =>
            {
                //Signin options
                option.SignIn.RequireConfirmedAccount = false;
                option.SignIn.RequireConfirmedEmail = true;
                option.SignIn.RequireConfirmedPhoneNumber = false;

                //Password options
                option.Password.RequireDigit = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequiredLength = 6;

                //Lockout options
                option.Lockout.AllowedForNewUsers = true;
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                option.Lockout.MaxFailedAccessAttempts = 3;

                //User options

                option.User.RequireUniqueEmail = true;
            });
        }

        public static void MigrateData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<EzioLearningDbContext>();
            context.Database.Migrate();
            new DataSeeder().SeedAsync(context).Wait();
        }
    }

}
