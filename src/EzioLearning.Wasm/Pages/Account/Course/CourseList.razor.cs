using Blazored.LocalStorage;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Models.Request;
using EzioLearning.Wasm.Services.Implement;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EzioLearning.Wasm.Pages.Account.Course
{
    public partial class CourseList : AccountComponentBase
    {
        private List<CourseItemViewDto> CourseItemViewDtos { get; set; } = [];
        [Inject] private ICourseService CourseService { get; set; } = default!;

        private CourseListOptions CourseListOptions { get; set; } = new();
        [Inject] private ICourseCategoryService CourseCategoryService { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        private List<CourseCategoryViewDto> CourseCategoryViewDtos { get; set; } = new();

        public int PageCount { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await FetchCoursesView();
            CourseCategoryViewDtos = await CourseCategoryService.GetCourseCategories();
        }

        private async Task OnPageNumberChanged(int pageValue)
        {
            CourseListOptions.PageNumber = pageValue;
            await FetchCoursesView();
        }

        private async Task FetchCoursesView()
        {
            var response = await CourseService.GetCourseByTeacher(AccountInfoMinimal.Id, CourseListOptions);

            PageCount = response.Data!.PageCount;
            CourseItemViewDtos = response.Data!.Data.ToList();
        }
        private async Task OnCourseCategoryChanged(ChangeEventArgs e)
        {
            var selectedCategoryId = Guid.Parse(e.Value?.ToString() ?? Guid.Empty.ToString());

            var selectedCategory = CourseCategoryViewDtos.FirstOrDefault(c => c.Id == selectedCategoryId);

            CourseListOptions.CourseCategoryIds.Clear();

            if (selectedCategory != null && selectedCategoryId != Guid.Empty)
            {
                CourseListOptions.CourseCategoryIds.Add(selectedCategoryId);
            }

            await FetchCoursesView();
        }

    }

}
