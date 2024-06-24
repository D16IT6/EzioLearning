using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;


namespace EzioLearning.Wasm.Pages.Course
{
    public partial class CourseDetail
    {
        [Parameter]public Guid CourseId { get; set; }
        [Inject] public ICourseService CourseService { get; set; } = default!;
        private CourseDetailViewDto CourseDetailViewDto { get; set; } = new();


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

        }
    }
}
