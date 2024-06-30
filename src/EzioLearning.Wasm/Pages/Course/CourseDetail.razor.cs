using System.Security.Claims;
using Blazored.LocalStorage;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Wasm.Components.Common;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;


namespace EzioLearning.Wasm.Pages.Course
{
    public partial class CourseDetail
    {
        [Parameter] public Guid CourseId { get; set; }
        [Inject] public ICourseService CourseService { get; set; } = default!;
        private CourseDetailViewDto CourseDetailViewDto { get; set; } = new();
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;
        [Inject] private ISnackbar SnackBar { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;

        [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;
        private AuthenticationState? AuthenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            AuthenticationState = await AuthenticationStateTask;

            CourseDetailViewDto = (await CourseService.GetCourseDetailPage(CourseId)).Data!;

        }

        private Task ShowPlayVideo(CourseLectureViewDto lecture)
        {
            var dialogParams = new DialogParameters { { nameof(VideoPreviewDialog.Lecture), lecture } };

            DialogService.Show<VideoPreviewDialog>(lecture.Name, dialogParams, new DialogOptions()
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Large
            });
            return Task.CompletedTask;
        }

        private async Task AddCourseToFavoriteList()
        {
            await CourseService.AddCourseToFavoriteList(CourseDetailViewDto.Id);
            SnackBar.Add("Thêm khoá học vào danh sách yêu thích thành công", Severity.Success);
        }

        private async Task OnClickBuyButton()
        {
            if (AuthenticationState == null) return;
            var userId = Guid.Parse(AuthenticationState.User.Claims.First(x => x.Type.Contains(ClaimTypes.PrimarySid)).Value);
            var response = await CourseService.BuyCourse(new CoursePaymentRequestDto()
            {
                CourseId = CourseDetailViewDto.Id,
                UserId = userId,
                Price = CourseDetailViewDto.Price - CourseDetailViewDto.PromotionPrice
            });
            if (response.IsSuccess)
            {
                var data = response.Data;
                if (data != null && !string.IsNullOrEmpty(data.Url))
                {
                    NavigationManager.NavigateTo(data.Url);
                }
            }
            else
            {
                SnackBar.Add(response.Message, Severity.Error);
            }

        }
    }
}
