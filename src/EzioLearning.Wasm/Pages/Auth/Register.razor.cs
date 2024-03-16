﻿using EzioLearning.Share.Dto.Auth;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net;
using EzioLearning.Wasm.Utils.Common;

namespace EzioLearning.Wasm.Pages.Auth;

public partial class Register
{

    private string[] AcceptTypes => FileConstants.AcceptTypes;

    [SupplyParameterFromQuery] private string? Email { get; set; } = string.Empty;

    [SupplyParameterFromQuery] private string? FirstName { get; set; } = string.Empty;
    [SupplyParameterFromQuery] private string? LastName { get; set; } = string.Empty;
    [SupplyParameterFromQuery] private string? UserName { get; set; } = string.Empty;

    [SupplyParameterFromQuery] private string? LoginProvider { get; set; } = string.Empty;
    [SupplyParameterFromQuery] private string? ProviderName { get; set; } = string.Empty;
    [SupplyParameterFromQuery] private string? ProviderKey { get; set; } = string.Empty;

    [SupplyParameterFromForm] public RegisterRequestClientDto RegisterModel { get; set; } = new();

    [Inject] private IJSRuntime JsRunTime { get; set; } = default!;
    private IJSObjectReference JsObjectReference { get; set; } = default!;


    private bool DisabledEmail { get; set; }

    [Inject] private ISnackbar SnackBar { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;
    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ITokenService TokenService { get; set; } = default!;

    private bool DisableSubmitButton { get; set; }
    private string SubmitButtonText { get; set; } = "Đăng ký";

    private IBrowserFile? File { get; set; }

    private Task LoadFile(InputFileChangeEventArgs e)
    {
        File = e.File;
        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        JsObjectReference = await JsRunTime.InvokeAsync<IJSObjectReference>("import",
            $"/{nameof(Pages)}/{nameof(Auth)}/{nameof(Register)}.razor.js");

        await JsObjectReference.InvokeVoidAsync("hideLabelInputDateMargin");
    }

    protected override void OnInitialized()
    {
        RegisterModel.Email = Email;
        RegisterModel.FirstName = FirstName;
        RegisterModel.LastName = LastName;
        RegisterModel.UserName = UserName;

        RegisterModel.LoginProvider = LoginProvider;
        RegisterModel.ProviderName = ProviderName;
        RegisterModel.ProviderKey = ProviderKey;

        if (!string.IsNullOrEmpty(Email))
        {
            DisabledEmail = true;
            SnackBar.Add("Do bạn đăng nhập từ bên thứ ba nên email không thể sửa đổi", Severity.Warning,
                option => { option.ActionColor = Color.Warning; });
        }

        if (!string.IsNullOrEmpty(ProviderName))
            SnackBar.Add($"Bạn đang sử dụng {ProviderName} để xác thực, hãy hoàn tất mẫu đăng ký nhé!", Severity.Info,
                option => { option.ActionColor = Color.Info; });
    }

    private async Task OnValidSubmitRegisterForm()
    {
        if (File != null)
        {
            var fileExtension = Path.GetExtension(File.Name);
            if (File.Size > FileConstants.UploadLimit)
            {
                SnackBar.Add(FileConstants.FileTooLargeMessage, Severity.Error, config => { config.ActionColor = Color.Warning; });
                return;
            }

            if (!AcceptTypes.Contains(fileExtension))
            {
                SnackBar.Add(FileConstants.FileNowAllowExtensionMessage, Severity.Error,
                    config => { config.ActionColor = Color.Warning; });
                RegisterModel.Avatar = null;
                return;
            }
        }


        SubmitButtonText = "Đang xử lý...";
        DisableSubmitButton = true;
        var data = await AuthService.Register(RegisterModel, File);

        switch (data!.Status)
        {
            case HttpStatusCode.BadRequest:

                DisableSubmitButton = false;
                SubmitButtonText = "Đăng ký";

                StateHasChanged();

                SnackBarService.ShowErrorFromResponse(data);

                File = null;
                break;

            case HttpStatusCode.OK:

                SubmitButtonText = "Đăng ký thành công!";
                StateHasChanged();

                await TokenService.SaveFromResponse(data);
                SnackBar.Add("Đăng ký tài khoản thành công!", Severity.Success, option =>
                {
                    option.ActionColor = Color.Success;
                    option.CloseAfterNavigation = true;
                });
                await Task.Delay(2000);
                NavigationManager.NavigateTo(RouteConstants.Home, true);
                break;
        }

        
    }
}