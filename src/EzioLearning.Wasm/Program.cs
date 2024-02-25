using Blazored.LocalStorage;
using EzioLearning.Wasm.Providers;
using EzioLearning.Wasm.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor;
using MudBlazor.Services;
namespace EzioLearning.Wasm
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");



            builder.Services.AddBlazorBootstrap();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddMudServices(config =>
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


            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();


            builder.Services.AddScoped(_ => new HttpClient
            {
                BaseAddress = new Uri(ApiConstants.BaseUrl),
                
            });
            
            await builder.Build().RunAsync();
        }
    }
}
