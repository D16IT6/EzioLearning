using System.Net;
using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Wasm.Providers;
using EzioLearning.Wasm.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Size = MudBlazor.Size;

namespace EzioLearning.Wasm.Pages.Auth
{
    public partial class Login
    {
        [SupplyParameterFromForm] public LoginRequestDto LoginRequest { get; set; } = new();

        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private ILogger<Login> Logger { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ISnackbar SnackBar { get; set; } = default!;

        private string GoogleCallbackUrl { get; set; } = string.Empty;
        private string FacebookCallbackUrl { get; set; } = string.Empty;

        public async Task LoginSubmit()
        {
            var result = await AuthService.Login(LoginRequest);
            Logger.LogInformation(result!.Message);

            if (result.Status == HttpStatusCode.OK)
            {
                SnackBar.Add(result.Message, Severity.Success, (configure) =>
                {
                    configure.ActionColor = Color.Success;
                    configure.CloseAfterNavigation = true;
                    configure.Icon = Icons.Material.Filled.Login;
                    configure.IconColor = Color.Success;
                    configure.IconSize = Size.Large;
                });

                await Task.Delay(2000);

                NavigationManager.NavigateTo(RouteConstants.Home, true);
            }
            else
            {
                SnackBar.Add(result.Message, Severity.Error, configure =>
                {
                    configure.ActionColor = Color.Error;
                    configure.Icon = Icons.Material.Filled.Login;
                    configure.IconColor = Color.Error;
                    configure.IconSize = Size.Large;
                });
            }
        }
        protected override void OnInitialized()
        {
            GoogleCallbackUrl = 
                $"{ApiConstants.BaseUrl}api/Auth/GoogleLogin?returnUrl=" +
                NavigationManager.ToAbsoluteUri("/ExternalLogin").AbsoluteUri;
            base.OnInitialized();
            
            FacebookCallbackUrl = 
                $"{ApiConstants.BaseUrl}api/Auth/FacebookLogin?returnUrl=" +
                NavigationManager.ToAbsoluteUri("/ExternalLogin").AbsoluteUri;
            base.OnInitialized();
        }
    }
}
