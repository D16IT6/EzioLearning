using EzioLearning.Share.Dto.Learning.CourseCategory;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Components.Home;

public partial class HomeTopCategoryItem
{
    [CascadingParameter] private TopCourseCategoryDto TopCourseCategory { get; set; } = default!;
}