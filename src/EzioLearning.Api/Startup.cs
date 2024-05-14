using EzioLearning.Api.Authorization;
using EzioLearning.Api.Middleware;
using EzioLearning.Api.Models.Auth;
using EzioLearning.Api.Models.Payment;
using EzioLearning.Api.Services;
using EzioLearning.Core.Dto;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Core.Validators;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Infrastructure.Repositories.Learning;
using EzioLearning.Infrastructure.SeedWorks;
using EzioLearning.Share.Models.Token;
using EzioLearning.Share.Utils;
using EzioLearning.Share.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Net.payOS;
using Serilog;
using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.ResponseCompression;

namespace EzioLearning.Api;

internal static class Startup
{
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
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/TestHub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            }).AddGoogle(options =>
            {
                options.ClientId = externalAuthentication.Google.ClientId;
                options.ClientSecret = externalAuthentication.Google.ClientSecret;
            }).AddFacebook(options =>
            {
                options.ClientId = externalAuthentication.Facebook.ClientId;
                options.ClientSecret = externalAuthentication.Facebook.ClientSecret;
            }).AddMicrosoftAccount(options =>
            {
                options.ClientId = externalAuthentication.Microsoft.ClientId;
                options.ClientSecret = externalAuthentication.Microsoft.ClientSecret;
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

    private static async Task MigrateData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        await using var context = scope.ServiceProvider.GetRequiredService<EzioLearningDbContext>();
        await context.Database.MigrateAsync();
        await new DataSeeder().SeedAsync(context);
    }

    private static void ConfigurePayments(this IServiceCollection services, IConfiguration configuration)
    {
        var paymentSettings = new PaymentSettings();
        configuration.Bind(nameof(PaymentSettings), paymentSettings);

        services.AddSingleton(_ =>
        {
            var paypal = new PaypalClient(paymentSettings.Paypal);
            return paypal;
        });

        services.AddSingleton(_ =>
        {
            var payos = paymentSettings.Payos;

            return new PayOS(payos.ClientId, payos.ApiKey, payos.ChecksumKey);
        });

    }

    private static void ConfigureLogs(this IServiceCollection services, IConfiguration _)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
            {
                //loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            }
        );
    }

    private static void ConfigureMultiLanguages(this IServiceCollection services, IConfiguration _)
    {
        var defaultCulture = new CultureInfo("vi-VN");

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new RequestCulture(defaultCulture);

            options.SupportedCultures = new List<CultureInfo>
            {
                defaultCulture,
                new("en-US")
            };

            options.SupportedUICultures = options.SupportedCultures;

            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new RouteDataRequestCultureProvider() { Options = options },
                new QueryStringRequestCultureProvider() { Options = options },
                new AcceptLanguageHeaderRequestCultureProvider() { Options = options }
            };
        });

        services.AddLocalization(options =>
        {
            options.ResourcesPath = "Resources";
        });
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
    }


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
                policyBuilder => { policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); }
            );
        });

        services.AddDbContext<EzioLearningDbContext>(option =>
        {
            option.UseSqlServer(configuration.GetConnectionString(nameof(EzioLearning)));
        });

        services.ConfigureValidator();

        services.ConfigureIdentity();

        services.ConfigureRepository();

        services.AddAutoMapper(typeof(MapperClass));

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

        services.ConfigurePayments(configuration);

        services.ConfigureLogs(configuration);

        services.ConfigureMultiLanguages(configuration);

        services.AddResponseCompression(opts =>
        {
            opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                ["application/octet-stream"]);
        });

        services.AddSignalR(options =>
        {
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
        }).AddMessagePackProtocol();
    }

    internal static void Configure(this WebApplication app)
    {

        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseSwagger();
        //    app.UseSwaggerUI();
        //}

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("CorsPolicy");

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRequestLocalization();

        app.UseResponseCompression();


        app.UseAuthentication();
        app.UseMiddleware<Custom401ResponseMiddleware>();
        app.UseMiddleware<Custom403ResponseMiddleware>();
        app.UseAuthorization();

        app.MapControllers();

        app.UseMiddleware<HandleExceptionMiddleware>();

        app.MigrateData().Wait();
    }
}