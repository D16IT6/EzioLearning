using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Components.Account;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Net.Http.Json;
using System.Text.Json;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.Extensions.Localization;


namespace EzioLearning.Wasm.Pages.Account
{
    public partial class ChangeEmailConfirm : AccountComponentBase
    {
        [SupplyParameterFromQuery] public string UserId { get; set; } = string.Empty;
        [SupplyParameterFromQuery] public string Email { get; set; } = string.Empty;
        [SupplyParameterFromQuery] public string VerifyCode { get; set; } = string.Empty;
        public ChangeEmailConfirmDto ChangeEmailConfirmDto { get; set; } = new();
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private ITokenService TokenService { get; set; } = default!;
        [Inject] private IStringLocalizer<ChangeEmailConfirm> Localizer { get; set; } = default!;
        private bool Loading { get; set; } = true;
        private string LoadingText { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            LoadingText = Localizer.GetString("LoadingText");
            ChangeEmailConfirmDto = new ChangeEmailConfirmDto()
            {
                Email = Email,
                VerifyCode = VerifyCode,
                UserId = UserId
            };

            await Task.Delay(1000);

            var response = await HttpClient.PutAsJsonAsync("/api/Account/ChangeEmailConfirm", ChangeEmailConfirmDto);
            await using var stream = await response.Content.ReadAsStreamAsync();
            var responseData =
                await JsonSerializer.DeserializeAsync<ResponseBase>(stream, JsonCommonOptions.DefaultSerializer);

            Loading = false;
            if (responseData!.IsSuccess)
            {
                LoadingText = Localizer.GetString("LoadingTextSuccess");
                StateHasChanged();
                await TokenService.DeleteToken();
                await NavigationService.Navigate(RouteConstants.Login, responseData.Message, 1, forceLoad: true,
                    Severity.Success);
            }
            else
            {
                SnackBarService.ShowErrorFromResponse(responseData);
                LoadingText = Localizer.GetString("LoadingTextFail");
                StateHasChanged();

            }
        }
    }
}
