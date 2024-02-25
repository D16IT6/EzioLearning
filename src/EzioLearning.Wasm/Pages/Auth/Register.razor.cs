using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Wasm.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Net;

namespace EzioLearning.Wasm.Pages.Auth
{
    public partial class Register
    {
        private string[] AcceptTypes { get; set; } = [".png", ".bmp", ".jpg", ".jpeg"];

        [SupplyParameterFromQuery] private string? Email { get; set; } = String.Empty;

        [SupplyParameterFromQuery] private string? FirstName { get; set; } = String.Empty;
        [SupplyParameterFromQuery] private string? LastName { get; set; } = String.Empty;
        [SupplyParameterFromQuery] private string? UserName { get; set; } = String.Empty;

        [SupplyParameterFromQuery] private string? LoginProvider { get; set; } = String.Empty;
        [SupplyParameterFromQuery] private string? ProviderName { get; set; } = String.Empty;
        [SupplyParameterFromQuery] private string? ProviderKey { get; set; } = String.Empty;

        [SupplyParameterFromForm]
        public RegisterRequestClientDto RegisterModel { get; set; } = new();

        private bool DisabledEmail { get; set; }

        [Inject] private ILogger<Register> Logger { get; set; } = default!;

        [Inject] private ISnackbar SnackBar { get; set; } = default!;
        [Inject] private IAuthService AuthService { get; set; } = default!;
        [Inject] private ITokenService TokenService { get; set; } = default!;


        private const long UploadLimit = 10 * 1024 * 1024;
        private const string FileTooLargeMessage = "File quá lớn, không được phép!";
        private const string FileNowAllowExtensionMessage = "Không cho phép file định dạng này!";

        private IBrowserFile? File { get; set; }
        private Task LoadFile(InputFileChangeEventArgs e)
        {
            var file = e.File;
            if (file.Size > 0)
            {
                var fileExtension = Path.GetExtension(file.Name);
                if (file.Size > UploadLimit)
                {
                    SnackBar.Add(FileTooLargeMessage, Severity.Error, config =>
                    {
                        config.ActionColor = Color.Warning;
                    });
                }
                if (!AcceptTypes.Contains(fileExtension))
                {
                    SnackBar.Add(FileNowAllowExtensionMessage, Severity.Error, config =>
                    {
                        config.ActionColor = Color.Warning;
                    });
                    RegisterModel.Avatar = null;
                    return Task.CompletedTask;
                }
                else
                {
                    File = file;
                }
            }

            return Task.CompletedTask;
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
                SnackBar.Add("Do bạn đăng nhập từ bên thứ ba nên email không thể sửa đổi", Severity.Warning, option =>
                {
                    option.ActionColor = Color.Warning;
                });
            }

            if (!string.IsNullOrEmpty(ProviderName))
            {
                SnackBar.Add($"Bạn đang sử dụng {ProviderName} để xác thực, hãy hoàn tất mẫu đăng ký nhé!", Severity.Info, option =>
                {
                    option.ActionColor = Color.Info;
                });
            }
        }

        private async Task OnValidSubmitRegisterForm()
        {
            var data = await AuthService.Register(RegisterModel, File);

            if (data!.Errors.Any())
            {
                foreach (var dataError in data.Errors)
                {
                    if (dataError.Value.Any())
                    {
                        foreach (var error in dataError.Value)
                        {
                            SnackBar.Add(error, Severity.Warning, option =>
                            {
                                option.ActionColor = Color.Warning;
                            });
                        }
                    }
                }
            }

            switch (data.Status)
            {
                case HttpStatusCode.BadRequest:

                    break;
                case HttpStatusCode.OK:

                    await TokenService.SaveFromResponse(data);
                    break;
            }

            Logger.LogInformation(data.Message);
        }
    }
}
