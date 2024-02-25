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
        [Inject] private ISnackBarService SnackBarService { get; set; } = default!;

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
                SnackBarService.ShowErrorFromResponse(result);
            }
        }
    }
}
