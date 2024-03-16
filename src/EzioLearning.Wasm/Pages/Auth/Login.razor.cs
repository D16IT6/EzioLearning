using EzioLearning.Share.Dto.Auth;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using EzioLearning.Wasm.Utils.Common;

namespace EzioLearning.Wasm.Pages.Auth;

public partial class Login
{
    [SupplyParameterFromForm] public LoginRequestDto LoginRequest { get; set; } = new();

    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ISnackbar SnackBar { get; set; } = default!;
    [Inject] private ITokenService TokenService { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;

    public async Task LoginSubmit()
    {
        var result = await AuthService.Login(LoginRequest);

        if (result.Status == HttpStatusCode.OK)
        {
            SnackBar.Add(result.Message, Severity.Success, configure =>
            {
                configure.ActionColor = Color.Success;
                configure.CloseAfterNavigation = true;
            });
            await TokenService.SaveToken(result.Data!);

            await Task.Delay(2000);

            NavigationManager.NavigateTo(RouteConstants.Home, true);
        }
        else
        {
            SnackBarService.ShowErrorFromResponse(result);
        }
    }
}