using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Utils;
using EzioLearning.Wasm.Authorization;
using EzioLearning.Wasm.Hubs;
using EzioLearning.Wasm.Pages;
using EzioLearning.Wasm.Providers;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Services;
using MudExtensions.Services;
using Serilog;
using Syncfusion.Blazor;
using ILogger = Serilog.ILogger;

namespace EzioLearning.Wasm
{
    public static class Startup
    {
        public static Task ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddSyncfusionBlazor();

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

            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();

            services.AddAuthorizationCore();



            services.AddCascadingAuthenticationState();

            services.ScanLocalServices();

            services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

            services.AddMudExtensions();


            services.AddScoped(_ =>
            {
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(ApiConstants.BaseUrl)

                };
                return httpClient;
            });

            using var log = new LoggerConfiguration()
                .WriteTo.File("Logs/log.txt")
                .CreateLogger();


            services.AddLocalization();

            services.ConfigureMultiLanguages();


            services.AddSingleton<ILogger>(log);

            services.AddScoped<HubConnectionManager>();

            services.AddLogging();

            return Task.CompletedTask;
        }

        private static void ConfigureMultiLanguages(this IServiceCollection services)
        {
            var defaultCulture = new CultureInfo("vi-VN");

            services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
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

        public static async Task LoadCurrentCulture(this WebAssemblyHost host)
        {

            var jsInterop = host.Services.GetRequiredService<IJSRuntime>();
            var currentCultureName = await jsInterop.InvokeAsync<string?>("blazorCulture.get") ?? "vi-VN";

            var culture = new CultureInfo(currentCultureName);

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.DefaultThreadCurrentUICulture = culture;

            //Header accept language
            var httpClient = host.Services.GetRequiredService<HttpClient>();

            httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(currentCultureName));
        }

        public static async Task ConnectToHub(this WebAssemblyHost host)
        {

            var hubConnectionManager = host.Services.GetRequiredService<HubConnectionManager>();
            var localStorage = host.Services.GetRequiredService<ILocalStorageService>();

            var testHub = await hubConnectionManager.GetHubConnectionAsync(HubConnectionEndpoints.Test,needAuthenticate:false);

            if (testHub != null)
            {
                if (testHub.State == HubConnectionState.Disconnected)
                {
                    await testHub.StartAsync();
                }

                Debug.WriteLine($"Đã kết nối tới {nameof(HubConnectionEndpoints.Test)}");

                testHub.On<IEnumerable<UserDto>>("ReceiveUsers", async users =>
                {
                    await localStorage.SetItemAsync("Users", users);
                });

                await testHub.InvokeAsync("SendUsers");

            }
        }

    }
}
