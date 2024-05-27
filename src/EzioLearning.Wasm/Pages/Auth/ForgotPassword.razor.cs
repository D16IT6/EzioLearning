using EzioLearning.Share.Dto.Auth;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.Extensions.Localization;
using EzioLearning.Wasm.Utils.Extensions;

namespace EzioLearning.Wasm.Pages.Auth;

public partial class ForgotPassword : AuthComponentBase
{
    [SupplyParameterFromForm] public ForgotPasswordDto ForgotPasswordDto { get; set; } = new();

    [Inject] private ISnackbar SnackBar { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;
    [Inject] private IStringLocalizer<ForgotPassword> Localizer { get; set; } = default!;

    private bool DisableSubmitButton { get; set; }
    private string SubmitButtonText { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var confirmUrl = NavigationManager.ToAbsoluteUri(RouteConstants.ConfirmPassword);

        ForgotPasswordDto.ClientConfirmUrl = confirmUrl.AbsoluteUri;
    }


    public async Task SubmitForgotPasswordForm()
    {
        SubmitButtonText = Localizer.GetString("SubmitTextProcessing");
        DisableSubmitButton = true;

        var data = await AuthService.ForgotPassword(ForgotPasswordDto);

        switch (data!.Status)
        {
            case HttpStatusCode.BadRequest:

                DisableSubmitButton = false;

                StateHasChanged();
                SnackBarService.ShowErrorFromResponse(data);
                break;
            case HttpStatusCode.OK:
                SnackBar.Add(data.Message, Severity.Success, option =>
                {
                    option.ActionColor = Color.Success;
                    option.CloseAfterNavigation = true;
                });
                await Task.Delay(3000);
                NavigationManager.NavigateTo(RouteConstants.Index);
                break;
        }

        SubmitButtonText = Localizer.GetString("SubmitText");
        DisableSubmitButton = false;
        StateHasChanged();
    }
}