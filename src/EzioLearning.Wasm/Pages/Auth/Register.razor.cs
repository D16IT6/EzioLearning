using EzioLearning.Share.Dto.Auth;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.Extensions.Localization;
using EzioLearning.Wasm.Utils.Extensions;

namespace EzioLearning.Wasm.Pages.Auth;

public partial class Register : AuthComponentBase
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
    [Inject] private IStringLocalizer<Register> Localizer { get; set; } = default!;
    private IJSObjectReference JsObjectReference { get; set; } = default!;


    private bool DisabledEmail { get; set; }

    [Inject] private ISnackbar SnackBar { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;
    [Inject] private ITokenService TokenService { get; set; } = default!;

    private bool DisableSubmitButton { get; set; }
    private string SubmitButtonText { get; set; } = string.Empty;


    private Task LoadFile(InputFileChangeEventArgs e)
    {
        RegisterModel.BrowserFile = e.File;
        return Task.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        JsObjectReference = await JsRunTime.InvokeAsync<IJSObjectReference>("import",
            $"/{nameof(Pages)}/{nameof(Auth)}/{nameof(Register)}.razor.js");

        SubmitButtonText = Localizer.GetString("ButtonText");
        StateHasChanged();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender) return;
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
            SnackBar.Add(Localizer.GetString("ExternalLoginEmailMessage"), Severity.Warning,
                option => { option.ActionColor = Color.Warning; });
        }

        if (!string.IsNullOrEmpty(ProviderName))
            SnackBar.Add(Localizer.GetString("ExternalLoginProviderMessage",ProviderName), Severity.Info,
                option => { option.ActionColor = Color.Info; });
    }

    private async Task OnValidSubmitRegisterForm()
    {
        var file = RegisterModel.BrowserFile;
        if (file != null)
        {
            var fileExtension = Path.GetExtension(file.Name);
            if (file.Size > FileConstants.UploadLimit)
            {
                SnackBar.Add(FileConstants.FileTooLargeMessage, Severity.Error, config => { config.ActionColor = Color.Warning; });
                return;
            }

            if (!AcceptTypes.Contains(fileExtension))
            {
                SnackBar.Add(FileConstants.FileNowAllowExtensionMessage, Severity.Error,
                    config => { config.ActionColor = Color.Warning; });
                RegisterModel.BrowserFile = null;
                return;
            }
        }


        SubmitButtonText = Localizer.GetString("ButtonTextProcessing");
        DisableSubmitButton = true;
        var data = await AuthService.Register(RegisterModel);

        switch (data.Status)
        {
            case HttpStatusCode.BadRequest:

                DisableSubmitButton = false;
                SubmitButtonText = Localizer.GetString("ButtonText");

                StateHasChanged();

                SnackBarService.ShowErrorFromResponse(data);

                break;

            case HttpStatusCode.OK:

                SubmitButtonText = Localizer.GetString("ButtonTextSuccess"); 
                StateHasChanged();

                await TokenService.SaveFromResponse(data);
                SnackBar.Add(data.Message, Severity.Success, option =>
                {
                    option.ActionColor = Color.Success;
                    option.CloseAfterNavigation = true;
                });
                await Task.Delay(2000);
                NavigationManager.NavigateTo(RouteConstants.Index, true);
                break;
        }
    }
}