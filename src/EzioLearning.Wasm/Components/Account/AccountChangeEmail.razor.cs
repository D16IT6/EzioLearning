using EzioLearning.Share.Dto.Account;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Components.Common;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.Extensions.Localization;
using MudBlazor;
using EzioLearning.Wasm.Utils.Extensions;


namespace EzioLearning.Wasm.Components.Account
{
    public partial class AccountChangeEmail : AccountComponentBase
    {
        [Inject] private IStringLocalizer<AccountChangeEmail> Localizer { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [Inject] private ISnackbar SnackBar { get; set; } = default!;
        [Inject] private HttpClient HttpClient { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;
        private bool DisabledButton { get; set; }
        [SupplyParameterFromForm] public ChangeEmailDto AccountInfoChangeEmail { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var currentEmail = AccountInfoMinimal.Email;

            AccountInfoChangeEmail.CurrentEmail = AccountInfoChangeEmail.NewEmail = currentEmail!;

            AccountInfoChangeEmail.ClientUrl =
                NavigationManager.ToAbsoluteUri(RouteConstants.AccountRoute.ConfirmChangeEmail).AbsoluteUri;
        }
        private async Task OnSubmitChangeEmail()
        {
            var dialogParams = new DialogParameters<SimpleDialog>
            {
                {
                    x=>
                        x.ContentHtml,
                    Localizer.GetString(
                        "MessageBoxContent", 
                        AccountInfoChangeEmail.CurrentEmail,
                        AccountInfoChangeEmail.NewEmail)

                },
                {
                    x => x.CancelText,Localizer.GetString("MessageBoxCancelText")
                },
                {
                    x=>x.SubmitText ,Localizer.GetString("MessageBoxSubmitText")
                },
                {
                    x=>x.SubmitColor, Color.Success
                }
            };

            var result = (await DialogService.ShowAsync<SimpleDialog>(
                Localizer.GetString("MessageBoxTitle"), 
                dialogParams)
                );

            var x = await result.Result;
            if (!x.Canceled && (bool)x.Data)
            {
                DisabledButton = true;
                StateHasChanged();

                var response = await HttpClient.PutAsJsonAsync("/api/AccountRoute/ChangeEmail", AccountInfoChangeEmail);
                await using var stream = await response.Content.ReadAsStreamAsync();

                var responseData = await JsonSerializer.DeserializeAsync<ResponseBase>(stream, JsonCommonOptions.DefaultSerializer);

                if (responseData!.Status == HttpStatusCode.OK)
                {
                    SnackBar.Add(responseData.Message, Severity.Success);
                }
                else
                {
                    SnackBarService.ShowErrorFromResponse(responseData);
                }

                DisabledButton = false;

                StateHasChanged();

            }
        }
    }
}
