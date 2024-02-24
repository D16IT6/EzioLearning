using EzioLearning.Blazor.Client.Providers;
using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Core.Models.Response;
using EzioLearning.Core.Models.Token;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MudBlazor;

namespace EzioLearning.Wasm.Pages.Auth
{
    public partial class Register
    {
        private string[] AcceptTypes { get; set; } = [".png", ".bmp", ".jpg", ".jpeg"];

        [SupplyParameterFromQuery]
        public string? Email { get; set; }

        [SupplyParameterFromQuery]
        public string? FirstName { get; set; }
        [SupplyParameterFromQuery]
        public string? LastName { get; set; }


        [SupplyParameterFromForm]
        public RegisterRequestClientDto RegisterModel { get; set; } = new();

        private bool DisabledEmail { get; set; }

        [Inject] private ILogger<Register> Logger { get; set; } = default!;
        [Inject] private HttpClient HttpClient { get; set; } = default!;

        [Inject] private ISnackbar SnackBar { get; set; } = default!;

        private IBrowserFile? File { get; set; }
        private Task LoadFile(InputFileChangeEventArgs e)
        {
            if (e.FileCount > 0)
            {
                File = e.File;
            }

            return Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            RegisterModel.Email = Email;
            RegisterModel.FirstName = FirstName;
            RegisterModel.LastName = LastName;

            if (!string.IsNullOrEmpty(Email))
            {
                DisabledEmail = true;
                SnackBar.Add("Do bạn đăng nhập từ bên thứ ba nên email không thể sửa đổi", Severity.Warning, option =>
                {
                    option.ActionColor = Color.Warning;
                });
            }
        }

        private async Task OnValidSubmitRegisterForm()
        {
            var multipartContent = new MultipartFormDataContent();


            var key = $"model.{nameof(RegisterModel.FirstName)}";
            Logger.LogInformation("Key:" + key);
            if (File != null)
            {
                var fileContent = new StreamContent(File.OpenReadStream());
                multipartContent.Add(fileContent, $"{nameof(RegisterModel.Avatar)}", File.Name);
            }


            multipartContent.Add(new StringContent(RegisterModel.FirstName!), nameof(RegisterModel.FirstName));
            multipartContent.Add(new StringContent(RegisterModel.LastName!), nameof(RegisterModel.LastName));
            multipartContent.Add(new StringContent(RegisterModel.UserName!), nameof(RegisterModel.UserName));
            multipartContent.Add(new StringContent(RegisterModel.Password!), nameof(RegisterModel.Password));
            multipartContent.Add(new StringContent(RegisterModel.ConfirmPassword!), nameof(RegisterModel.ConfirmPassword));
            multipartContent.Add(new StringContent(RegisterModel.PhoneNumber!), nameof(RegisterModel.PhoneNumber));
            multipartContent.Add(new StringContent(RegisterModel.Email!), nameof(RegisterModel.Email));
            multipartContent.Add(new StringContent(RegisterModel.DateOfBirth.ToString("yyyy-MM-dd")), nameof(RegisterModel.DateOfBirth));

            var response = await HttpClient.PostAsync("api/Auth/Register", multipartContent);

            await using var stream = await response.Content.ReadAsStreamAsync();

            ResponseBase? data = null;
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:

                    data = await JsonSerializer.DeserializeAsync<ResponseBase>(stream, JsonCommonOptions.DefaultSerializer);
                    break;
                case HttpStatusCode.OK:

                    data = await JsonSerializer.DeserializeAsync<ResponseBaseWithData<TokenResponse>>(stream, JsonCommonOptions.DefaultSerializer);
                    break;
            }

            Logger.LogInformation(data!.Message);
        }
    }
}
