using EzioLearning.Share.Dto.Auth;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Pages.Auth;

public partial class Login : AuthComponentBase
{
    [SupplyParameterFromForm] public LoginRequestDto LoginRequest { get; set; } = new();

    [Inject] private ISnackbar SnackBar { get; set; } = default!;
    [Inject] private ITokenService TokenService { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;

    [Inject] private IStringLocalizer<Login> Localizer { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateTask;
        if (authState.User.Identity is { IsAuthenticated: true })
        {
            NavigationManager.NavigateTo(RouteConstants.Index);
        }
    }

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

            NavigationManager.NavigateTo(RouteConstants.Index, true);
        }
        else
        {
            SnackBarService.ShowErrorFromResponse(result);
        }
    }
}