using EzioLearning.Share.Dto.Auth;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Wasm.Pages.Auth;

public partial class ConfirmPassword
{
    [SupplyParameterFromForm] private ConfirmPasswordDto ConfirmPasswordDto { get; set; } = new();

    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private ISnackbar SnackBar { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private IStringLocalizer<ConfirmPassword> Localizer { get; set; } = default!;

    [SupplyParameterFromQuery] private string VerifyCode { get; set; } = string.Empty;

    [SupplyParameterFromQuery] private string Email { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        ConfirmPasswordDto.VerifyCode = VerifyCode;
        ConfirmPasswordDto.Email = Email;
    }

    public async Task OnConfirmPasswordSubmit()
    {
        var data = await AuthService.ConfirmPassword(ConfirmPasswordDto);

        switch (data!.Status)
        {
            case HttpStatusCode.BadRequest:
                SnackBarService.ShowErrorFromResponse(data);
                break;
            case HttpStatusCode.OK:
                SnackBar.Add(data.Message, Severity.Success, option =>
                {
                    option.ActionColor = Color.Success;
                    option.CloseAfterNavigation = true;
                });
                await Task.Delay(3000);

                NavigationManager.NavigateTo(RouteConstants.Login);
                break;
        }
    }
}