using System.Text.Json.Serialization;
using EzioLearning.Api.Authorization;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Middleware;
using EzioLearning.Api.Models.Auth;
using EzioLearning.Api.Services;
using EzioLearning.Core.Dto;
using EzioLearning.Core.Repositories;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Core.Validators;
using EzioLearning.Domain.Common;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.Repositories;
using EzioLearning.Infrastructure.SeedWorks;
using EzioLearning.Share.Dto;
using EzioLearning.Share.Models.Token;
using EzioLearning.Share.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EzioLearning.Api;

internal static class Startup
{
    internal static void ConfigureBuilder(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;


        services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddCors(option =>
        {
            option.AddPolicy("CorsPolicy",
                policyBuilder => { policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
        });

        services.AddDbContext<EzioLearningDbContext>(option =>
        {
            option.UseSqlServer(configuration.GetConnectionString(ConnectionConstants.ConnectionStringName));
        });

        services.ConfigureValidator();

        services.ConfigureIdentity();

        services.ConfigureRepository();

        services.AddAutoMapper([typeof(MapperClass), typeof(MapperShareClass)]);

        services.ConfigureAuthentication(configuration);


        services.ConfigureAuthorization(configuration);

        services.AddTransient(_ =>
        {
            var jwtConfiguration = new JwtConfiguration
            {
                PrivateKey = "Default if cannot bind configuration @#!^%&"
            };
            configuration.Bind(nameof(JwtConfiguration), jwtConfiguration);
            return jwtConfiguration;
        });


        services.AddMemoryCache();

        services.ConfigureLocalService(configuration);

        services.ConfigureCustomMiddleware();
    }



    private static void ConfigureLocalService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<CacheService>();
        services.AddTransient<JwtService>();
        services.AddTransient<FileService>();
        services.AddTransient<PermissionService>();

        services.AddSingleton(_ =>
        {
            MailSettings mailSettings = new();
            configuration.Bind(nameof(mailSettings), mailSettings);
            return mailSettings;
        });
        services.AddTransient<MailService>();
    }

    private static void ConfigureIdentity(this IServiceCollection services)
    {
        services.AddIdentity<AppUser, AppRole>()
            .AddEntityFrameworkStores<EzioLearningDbContext>()
            .AddDefaultTokenProviders();


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
            option.Lockout.AllowedForNewUsers = false;
            option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            option.Lockout.MaxFailedAccessAttempts = 3;

            //User options

            option.User.RequireUniqueEmail = true;
        });
        services.Configure<DataProtectionTokenProviderOptions>(o =>
            o.TokenLifespan = TimeSpan.FromHours(2)); //limit verify code time
    }

    private static void ConfigureValidator(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblies([
            typeof(ValidatorInjectClass).Assembly, typeof(ValidatorInjectShareClass).Assembly
        ]);

        services.AddFluentValidationAutoValidation();

        //services.AddFluentValidationClientsideAdapters();
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
            var directInterface =
                allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).FirstOrDefault();
            if (directInterface != null)
                services.Add(new ServiceDescriptor(directInterface, repositoryService, ServiceLifetime.Scoped));
        }

        services.AddScoped<ICourseCategoryRepository, CourseCategoryRepository>();
    }


    private static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtConfiguration = new JwtConfiguration
        {
            PrivateKey = "Default if cannot bind configuration @#!^%&"
        };
        configuration.Bind(nameof(JwtConfiguration), jwtConfiguration);

        //https://github.com/dotnet/aspnetcore/issues/9039

        var externalAuthentication = new Authentication();

        configuration.Bind("Authentication", externalAuthentication);

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;

                //external login
                options.TokenHandlers.Add(new GoogleJwtTokenHandler(externalAuthentication.Google.ClientId));
            }).AddGoogle(options =>
            {
                options.ClientId = externalAuthentication.Google.ClientId;
                options.ClientSecret = externalAuthentication.Google.ClientSecret;
            }).AddFacebook(options =>
            {
                options.ClientId = externalAuthentication.Facebook.ClientId;
                options.ClientSecret = externalAuthentication.Facebook.ClientSecret;
            });

        services.AddAuthorization();
    }

    private static void ConfigureAuthorization(this IServiceCollection services, IConfiguration _)
    {
        services.AddAuthorization(config =>
        {
            config.AddPolicy("AdminOnly", policy => { policy.RequireRole(RoleConstants.Admin); });
        });
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
    }

    private static void ConfigureCustomMiddleware(this IServiceCollection services)
    {
        services.AddScoped<Custom401ResponseMiddleware>();
        services.AddScoped<Custom403ResponseMiddleware>();
        services.AddScoped<HandleExceptionMiddleware>();
    }

    private static void MigrateData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<EzioLearningDbContext>();
        context.Database.Migrate();
        new DataSeeder().SeedAsync(context).Wait();
    }



    internal static void Configure(this WebApplication app)
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

        app.UseMiddleware<Custom401ResponseMiddleware>();
        app.UseMiddleware<Custom403ResponseMiddleware>();

        app.UseAuthorization();

        app.MapControllers();

        app.UseMiddleware<HandleExceptionMiddleware>();

        //app.MigrateData();
    }
}