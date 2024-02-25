using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Wasm.Services;
using EzioLearning.Wasm.Providers;
using MudBlazor;
using System.Net;


namespace EzioLearning.Wasm.Pages.Auth
{
    public partial class ConfirmPassword
    {
        [SupplyParameterFromForm]
        private ConfirmPasswordDto ConfirmPasswordDto { get; set; } = new();

        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private ISnackbar SnackBar { get; set; } = default!;
        [Inject] private ISnackBarService SnackBarService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [SupplyParameterFromQuery]
        private string VerifyCode { get; set; } = String.Empty;
        [SupplyParameterFromQuery]
        private string Email { get; set; } = String.Empty;

        protected override void OnInitialized()
        {
            ConfirmPasswordDto.VerifyCode = VerifyCode;
            ConfirmPasswordDto.Email = Email;
        }

        public async Task OnConfirmPasswordSubmit()
        {
            var data = await AuthService.ConfirmPassword(ConfirmPasswordDto);

            switch (data!.Status)
            {
                case HttpStatusCode.BadRequest:
                    SnackBarService.ShowErrorFromResponse(data);
                    break;
                case HttpStatusCode.OK:
                    SnackBar.Add(data.Message, Severity.Success, option =>
                    {
                        option.ActionColor = Color.Success;
                        option.CloseAfterNavigation = true;
                    });
                    await Task.Delay(3000);

                    NavigationManager.NavigateTo(RouteConstants.Login);
                    break;
            }
        }
    }
}
