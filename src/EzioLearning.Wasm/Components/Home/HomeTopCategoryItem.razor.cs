using Microsoft.AspNetCore.Components;
using EzioLearning.Core.Dtos.Learning.CourseCategory;


namespace EzioLearning.Wasm.Components.Home
{
    public partial class HomeTopCategoryItem
    {
        [CascadingParameter] private TopCourseCategoryDto TopCourseCategory { get; set; } = default!;

    }
}
