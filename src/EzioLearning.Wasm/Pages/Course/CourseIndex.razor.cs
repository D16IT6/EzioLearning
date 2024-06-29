using Blazored.LocalStorage;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Models.Request;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudExtensions;

namespace EzioLearning.Wasm.Pages.Course
{
    public partial class CourseIndex
    {
        private List<CourseItemViewDto> CourseItemViewDtos { get; set; } = new();

        [Inject] private ICourseService CourseService { get; set; } = default!;
        [Inject] private ICourseCategoryService CourseCategoryService { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        private List<CourseCategoryViewDto> CourseCategoryViewDtos { get; set; } = new();
        private CourseListOptions CourseListOptions { get; set; } = new();

        public int PageCount { get; set; }
        public CourseViewEnum CurrentCourseViewEnum { get; set; } = CourseViewEnum.Grid;

        protected override async Task OnInitializedAsync()
        {
            CourseCategoryViewDtos = await CourseCategoryService.GetCourseCategories();
            await base.OnInitializedAsync();

            CurrentCourseViewEnum =
                await LocalStorageService.GetItemAsync<CourseViewEnum?>(nameof(CurrentCourseViewEnum)) ?? CourseViewEnum.Grid;

            await FetchCoursesView();

        }


        private async Task FetchCoursesView()
        {
            var response = await CourseService.GetCoursePage(CourseListOptions);
            CourseItemViewDtos = response.Data!.Data.ToList();
            PageCount = response.Data.PageCount;
        }
        private async Task ChangeView(CourseViewEnum courseViewEnum)
        {
            if (courseViewEnum != CurrentCourseViewEnum)
            {
                CurrentCourseViewEnum = courseViewEnum;
            }
            await LocalStorageService.SetItemAsync(nameof(CurrentCourseViewEnum), CurrentCourseViewEnum);
        }

        private async Task OnPageNumberChanged(int value)
        {
            CourseListOptions.PageNumber = value;
            await FetchCoursesView();
        }

        private async Task OnSelectCategoryChanged(CourseCategoryViewDto obj)
        {
            if (CourseListOptions.CourseCategoryIds.Contains(obj.Id))
            {
                CourseListOptions.CourseCategoryIds.Remove(obj.Id);
            }
            else
            {
                CourseListOptions.CourseCategoryIds.Add(obj.Id);
            }
            await FetchCoursesView();
        }

        private async Task OnPriceTypeChanged(PriceType priceType)
        {
            if (CourseListOptions.PriceType != priceType)
            {
                CourseListOptions.PriceType = priceType;
                await FetchCoursesView();
            }
        }

        private async Task ChangeSearchText(ChangeEventArgs e)
        {
            CourseListOptions.SearchText = e.Value?.ToString();

            await FetchCoursesView();
        }
    }
}
