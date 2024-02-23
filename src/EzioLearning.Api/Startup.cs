using EzioLearning.Api.Services;
using EzioLearning.Core.Dtos.Learning.Course;
using EzioLearning.Core.Dtos.Learning.CourseCategory;
using EzioLearning.Core.Dtos.User;
using EzioLearning.Core.Dtos.Validators;
using EzioLearning.Core.Models.Token;
using EzioLearning.Core.Repositories;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.Repositories;
using EzioLearning.Infrastructure.SeedWorks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

            services.ConfigureValidator();

			services.ConfigureIdentity();

			services.ConfigureRepository();

			services.AddAutoMapper(typeof(CourseCategoryCreateDto));

			services.ConfigureAuthentication(configuration);

			services.AddTransient((_) =>
			{
				var jwtConfiguration = new JwtConfiguration
				{
					PrivateKey = "Default if cannot bind configuration @#!^%&"
				};
				configuration.Bind(nameof(JwtConfiguration), jwtConfiguration);
				return jwtConfiguration;
			});


			services.AddMemoryCache();

			services.AddSingleton<CacheService>();
			services.AddTransient<JwtService>();
			services.AddTransient<FileService>();
		}

		public static void ConfigureIdentity(this IServiceCollection services)
		{
			services.AddIdentity<AppUser, AppRole>()
				.AddEntityFrameworkStores<EzioLearningDbContext>()
				.AddApiEndpoints();


			services.Configure<IdentityOptions>(option =>
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

        private static void ConfigureValidator(this IServiceCollection services)
        {
            services
                .AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssembly(typeof(ValidatorInjectClass).Assembly);
        }


        private static void ConfigureRepository(this IServiceCollection services)
		{

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

            services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
        }


		public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			var jwtConfiguration = new JwtConfiguration
			{
				PrivateKey = "Default if cannot bind configuration @#!^%&"
			};
			configuration.Bind(nameof(JwtConfiguration), jwtConfiguration);

			//https://github.com/dotnet/aspnetcore/issues/9039

			services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = jwtConfiguration.Issuer,
						ValidAudience = jwtConfiguration.Audience,
						IssuerSigningKey = new JwtService(jwtConfiguration).SecurityKey
					};
				});
			services.AddAuthorization();
		}

		public static void MigrateData(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();
			using var context = scope.ServiceProvider.GetRequiredService<EzioLearningDbContext>();
			context.Database.Migrate();
			new DataSeeder().SeedAsync(context).Wait();
		}
		public static void Configure(this WebApplication app)
		{
            ConnectionConstants.ConnectionStringName = ConnectionStringName.Production;

            if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
				ConnectionConstants.ConnectionStringName = ConnectionStringName.Development;
			}


			app.UseCors("CorsPolicy");

			app.UseHttpsRedirection();

			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseAuthorization();

			app.MapControllers();

			app.MigrateData();

		}

	}

}
