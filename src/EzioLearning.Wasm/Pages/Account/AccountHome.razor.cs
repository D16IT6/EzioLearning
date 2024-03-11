using EzioLearning.Share.Dto.Account;
using EzioLearning.Wasm.Common;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Net;
using Microsoft.AspNetCore.Components.Forms;
using EzioLearning.Share.Models.Response;
using Microsoft.JSInterop;

namespace EzioLearning.Wasm.Pages.Account;

public partial class AccountHome
{
    [SupplyParameterFromForm] private AccountInfoDto AccountInfo { get; set; } = new();
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [Inject] private ISnackbar Snackbar { get; set; } = default!;
    [Inject] private ISnackBarService SnackBarService { get; set; } = default!;
    [Inject] private IAccountService AccountService { get; set; } = default!;
    [Inject] private ISnackbar SnackBar { get; set; } = default!;
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
    private IJSObjectReference JsObjectReference { get; set; } = default!;

    private IBrowserFile? Avatar { get; set; }

    [CascadingParameter] private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadJs();
        var authenticationState = await AuthenticationStateTask!;
        var isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;

        if (isAuthenticated)
        {
            var accountDto = await AccountService.GetInfo();
            switch (accountDto.Status)
            {
                case HttpStatusCode.BadRequest:
                    SnackBarService.ShowErrorFromResponse(accountDto);
                    await NavigateToHome();
                    break;
                case HttpStatusCode.Unauthorized:
                    await NavigateToHome("Bạn chưa xác thực");
                    break;
                case HttpStatusCode.Forbidden:
                    await NavigateToHome("Bạn không có quyền truy cập chức năng này");
                    break;
                case HttpStatusCode.OK:
                    AccountInfo = accountDto.Data!;
                    break;
            }
        }
    }

    private async Task LoadJs()
    {
        JsObjectReference = await JsRuntime.InvokeAsync<IJSObjectReference>(
            "import",
            $"/{nameof(Pages)}/{nameof(Account)}/{nameof(AccountHome)}.razor.js");
    }


    private async Task NavigateToHome(string message = "", int seconds = 0)
    {
        if (!string.IsNullOrEmpty(message))
            Snackbar.Add(message);

        await Task.Delay(TimeSpan.FromSeconds(seconds));
        NavigationManager.NavigateTo(RouteConstants.Home);
    }

    private async Task UpdateAccountInfo()
    {
        var response = await AccountService.UpdateInfo(AccountInfo);

        if (response!.Status == HttpStatusCode.OK)
        {
            Snackbar.Add("Cập nhật thông tin thành công", Severity.Success);
            //await Task.Delay(1000);
            //NavigationManager.NavigateTo(NavigationManager.Uri);

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
            SnackBar.Add("Cần chọn ảnh", Severity.Warning);
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
            Snackbar.Add("Cập nhật thông tin thành công", Severity.Success);
            AccountInfo = response.Data!;

            await JsObjectReference.InvokeVoidAsync("updateAvatar", ApiConstants.BaseUrl + AccountInfo.Avatar);
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