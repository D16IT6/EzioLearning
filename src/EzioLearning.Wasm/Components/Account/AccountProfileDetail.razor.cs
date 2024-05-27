using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System.Net;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.Extensions.Localization;
using EzioLearning.Wasm.Utils.Extensions;


namespace EzioLearning.Wasm.Components.Account
{
    public partial class AccountProfileDetail : AccountComponentBase
    {
        [SupplyParameterFromForm] private AccountInfoDto AccountInfo { get; set; } = new();
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private IStringLocalizer<AccountProfileDetail> Localizer { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        [Inject] private ISnackbar SnackBar { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
        private IJSObjectReference JsObjectReference { get; set; } = default!;

        private IBrowserFile? Avatar { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadJs();

            var accountInfoResponse = await AccountService.GetInfo();
            if (accountInfoResponse.Status == HttpStatusCode.OK)
            {
                AccountInfo = accountInfoResponse.Data!;
            }
            else
            {
                await NavigateToHome(accountInfoResponse.Message);
            }

        }
        private async Task LoadJs()
        {
            JsObjectReference = await JsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                $"/{nameof(Components)}/{nameof(Account)}/{nameof(AccountProfileDetail)}.razor.js");
            await JsObjectReference.InvokeVoidAsync("hideLabelInputDateMargin");
        }
        private async Task NavigateToHome(string? message = "", int seconds = 0)
        {
            await NavigationService.Navigate(RouteConstants.Index, message, seconds);
        }

        private async Task UpdateAccountInfo()
        {
            var response = await AccountService.UpdateInfo(AccountInfo);

            if (response!.Status == HttpStatusCode.OK)
            {
                Snackbar.Add(response.Message, Severity.Success);
                await JsObjectReference.InvokeVoidAsync("updateFullName", AccountInfo.FullName);
                AccountInfo = response.Data!;
            }
            else if (response.Status == HttpStatusCode.BadRequest)
            {
                SnackBarService.ShowErrorFromResponse(response);
            }
        }
        private async Task UpdateAvatar()
        {
            if (Avatar == null)
            {
                SnackBar.Add(Localizer.GetString("NeedAvatar"), Severity.Warning);
                return;
            }

            if (!HandleAcceptFile(Avatar))
            {
                return;
            }

            var response = await AccountService.UpdateAvatar(Avatar);

            await UpdatePage(response);


        }
        private async Task RemoveAvatar()
        {
            var response = await AccountService.UpdateAvatar();
            await UpdatePage(response);

        }

        private async Task UpdatePage(ResponseBaseWithData<AccountInfoDto>? response)
        {
            Avatar = null;
            if (response!.Status == HttpStatusCode.OK)
            {
                Snackbar.Add(response.Message, Severity.Success);
                AccountInfo = response.Data!;
                await JsObjectReference.InvokeVoidAsync("updateAvatar", ApiConstants.BaseUrl + AccountInfo.Avatar + $"?t={Guid.NewGuid()}");
                StateHasChanged();
            }
            else
            {
                SnackBarService.ShowErrorFromResponse(response);
            }
        }
        private void OnFileChange(InputFileChangeEventArgs e)
        {
            var file = e.File;

            if (HandleAcceptFile(file))
                Avatar = file;
        }

        private bool HandleAcceptFile(IBrowserFile file)
        {
            var fileExtension = Path.GetExtension(file.Name);
            if (file.Size > FileConstants.UploadLimit)
            {
                SnackBar.Add(FileConstants.FileTooLargeMessage, Severity.Error,
                    config => { config.ActionColor = Color.Warning; });
                return false;
            }

            if (FileConstants.AcceptTypes.Contains(fileExtension)) return true;

            SnackBar.Add(FileConstants.FileNowAllowExtensionMessage, Severity.Error,
                config => { config.ActionColor = Color.Warning; });
            return false;


        }
    }
}
