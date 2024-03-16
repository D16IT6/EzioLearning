using EzioLearning.Share.Dto.Account;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EzioLearning.Share.Models.Response;
using EzioLearning.Wasm.Components.Common;
using EzioLearning.Wasm.Utils.Common;
using MudBlazor;


namespace EzioLearning.Wasm.Components.Account
{
    public partial class AccountChangeEmail : AccountComponentBase
    {
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
                NavigationManager.ToAbsoluteUri(RouteConstants.Account.ConfirmChangeEmail).AbsoluteUri;
        }
        private async Task OnSubmitChangeEmail()
        {
            var dialogParams = new DialogParameters<SimpleDialog>
            {
                {
                    x=>
                        x.ContentHtml,
                    $"Bạn muốn đổi email từ <b>{AccountInfoChangeEmail.CurrentEmail}</b> " +
                    $"sang <b>{AccountInfoChangeEmail.NewEmail}</b> chứ?<br/>" +
                    $"Bạn cần xác thực trong email <b>{AccountInfoChangeEmail.CurrentEmail}</b>!"
                },
                {
                    x => x.CancelText,"Huỷ"
                },
                {
                    x=>x.SubmitText ,"Đồng ý"
                },
                {
                    x=>x.SubmitColor, Color.Success
                }
            };

            var result = (await DialogService.ShowAsync<SimpleDialog>("Xác nhận đổi email", dialogParams));

            var x = await result.Result;
            if (!x.Canceled && (bool)x.Data)
            {
                DisabledButton = true;
                StateHasChanged();

                var response = await HttpClient.PutAsJsonAsync("/api/Account/ChangeEmail", AccountInfoChangeEmail);
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
