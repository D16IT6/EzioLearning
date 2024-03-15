using Blazored.LocalStorage;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Providers;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using ILogger = Serilog.ILogger;

namespace EzioLearning.Wasm
{
    public static class Startup
    {
        public static async Task ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddBlazorBootstrap();
            services.AddBlazoredLocalStorage();
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;

                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 5000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });
            services.AddAuthorizationCore();
            services.AddCascadingAuthenticationState();

            services.ScanLocalServices();

            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();


            services.AddScoped(_ => new HttpClient
            {
                BaseAddress = new Uri(ApiConstants.BaseUrl)
            });

            await using var log = new LoggerConfiguration()
                .WriteTo.File("Logs/log.txt")
                .CreateLogger();

            services.AddSingleton<ILogger>(log);

        }

        private static void ScanLocalServices(this IServiceCollection services)
        {
            var localServices = typeof(IServiceBase).Assembly.GetTypes()
                .Where(x =>
                    x.GetInterfaces().Any(i => i.Name == nameof(IServiceBase))
                    && x is { IsAbstract: false, IsClass: true, IsGenericType: false }
                );

            foreach (var localService in localServices)
            {
                var allInterfaces = localService.GetInterfaces();
                var directInterface =
                    allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces())).FirstOrDefault();
                if (directInterface != null)
                    services.Add(new ServiceDescriptor(directInterface, localService, ServiceLifetime.Scoped));
            }
        }
    }
}
