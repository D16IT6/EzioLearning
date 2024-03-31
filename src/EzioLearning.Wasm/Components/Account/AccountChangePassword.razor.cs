using EzioLearning.Share.Dto.Account;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;

namespace EzioLearning.Wasm.Components.Account
{
    public partial class AccountChangePassword : AccountComponentBase
    {
        [SupplyParameterFromForm] public ChangePasswordDto ChangePassword { get; set; } = new();

        [Inject] private IStringLocalizer<AccountChangePassword> Localizer { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        private bool DisabledButton { get; set; } = false;

        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        private IJSObjectReference JsObjectReference { get; set; } = default!;


        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
        InputType _passwordInputType = InputType.Password;
        private bool IsPasswordShow { get; set; }

        private string _newPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        InputType _newPasswordInputType = InputType.Password;
        private bool IsNewPasswordShow { get; set; }

        private string _confirmPasswordInputIcon = Icons.Material.Filled.VisibilityOff;
        private InputType _confirmPasswordInputType = InputType.Password;
        private bool IsConfirmPasswordShow { get; set; }


        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private ITokenService TokenService { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
            await LoadJs();
        }
        private async Task LoadJs()
        {
            JsObjectReference = await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                $"/{nameof(Components)}/{nameof(Account)}/{nameof(AccountChangePassword)}.razor.js");

            await JsObjectReference.InvokeVoidAsync("hideMarginInput");
        }


        public async Task OnChangePassword()
        {
            DisabledButton = true;
            StateHasChanged();

            var responseData = await AccountService.ChangePassword(model: ChangePassword);

            if (responseData!.IsSuccess)
            {
                await TokenService.DeleteToken();
                await NavigationService.Navigate(RouteConstants.Login, responseData.Message, 2, true, Severity.Success);
            }
            else
            {
                SnackBarService.ShowErrorFromResponse(responseData);

            }

            DisabledButton = false;
            StateHasChanged();
        }


        private void TogglePassword(bool show, ref string inputIcon, ref InputType inputType)
        {
            if (IsPasswordShow)
            {
                IsPasswordShow = false;
                inputIcon = Icons.Material.Filled.VisibilityOff;
                inputType = InputType.Password;
            }
            else
            {
                IsPasswordShow = true;
                inputIcon = Icons.Material.Filled.Visibility;
                inputType = InputType.Text;
            }
        }
    }
}
