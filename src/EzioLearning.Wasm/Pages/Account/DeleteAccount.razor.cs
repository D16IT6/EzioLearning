using EzioLearning.Wasm.Components.Account;
using EzioLearning.Wasm.Components.Common;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace EzioLearning.Wasm.Pages.Account
{
    public partial class DeleteAccount :AccountComponentBase
    {
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private ITokenService TokenService { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

        }

        private async Task OnDeleteAccount()
        {
            var dialogParams = new DialogParameters<SimpleDialog>
            {
                {
                    x=>
                        x.ContentHtml,
                    $"Bạn thực sự muốn xoá tài khoản chứ?<br/>"+
                    "Nếu muốn khôi phục, bạn có thể liên hệ admin trước 30 ngày!"
                },
                {
                    x => x.CancelText,"Huỷ"
                },
                {
                    x=>x.SubmitText ,"Đồng ý"
                },
                {
                    x=>x.SubmitColor, Color.Error
                }
            };

            var result = await (await DialogService.ShowAsync<SimpleDialog>("Xoá tài khoản", dialogParams)).Result;

            if (!result.Canceled && (bool)result.Data)
            {
                var response = await AccountService.Delete();
                if (response!.IsSuccess)
                {
                    await TokenService.DeleteToken();
                    await NavigationService.Navigate(RouteConstants.Login, response.Message, 2, true, Severity.Success);
                }

            }
        }
    }
}
