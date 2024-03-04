﻿using EzioLearning.Share.Dto.Auth;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Services;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net;

namespace EzioLearning.Wasm.Pages.Auth;

public partial class ForgotPassword
{
    [SupplyParameterFromForm] public ForgotPasswordDto ForgotPasswordDto { get; set; } = new();

    [Inject] private IAuthService AuthService { get; set; } = default!;

    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ISnackbar SnackBar { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;
    private bool DisableSubmitButton { get; set; }
    private string SubmitButtonText { get; set; } = "Xác nhận";

    protected override void OnInitialized()
    {
        var confirmUrl = NavigationManager.ToAbsoluteUri(RouteConstants.ConfirmPassword);

        ForgotPasswordDto.ClientConfirmUrl = confirmUrl.AbsoluteUri;
    }

    public async Task SubmitForgotPasswordForm()
    {
        SubmitButtonText = "Đang xử lý...";
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
                NavigationManager.NavigateTo(RouteConstants.Home);
                break;
        }

        SubmitButtonText = "Xác nhận";
        StateHasChanged();
    }
}