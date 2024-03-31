using EzioLearning.Wasm.Components.Account;
using EzioLearning.Wasm.Components.Common;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;


namespace EzioLearning.Wasm.Pages.Account
{
    public partial class DeleteAccount :AccountComponentBase
    {
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private ITokenService TokenService { get; set; } = default!;
        [Inject] private IStringLocalizer<DeleteAccount> Localizer { get; set; } = default!;
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
                    Localizer.GetString("ContentHtml")
                },
                {
                    x => x.CancelText,Localizer.GetString("CancelText")
                },
                {
                    x=>x.SubmitText ,Localizer.GetString("SubmitText")
                },
                {
                    x=>x.SubmitColor, Color.Error
                }
            };

            var result = await (await DialogService.ShowAsync<SimpleDialog>(Localizer.GetString("Title"), dialogParams)).Result;

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
