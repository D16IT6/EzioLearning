using EzioLearning.Share.Dto.Learning.Course;
using Microsoft.AspNetCore.Components;

namespace EzioLearning.Wasm.Components.Course
{
	public partial class CourseInGrid
	{
		[CascadingParameter] public CourseItemViewDto CourseItemViewDto { get; set; } = new();

	}
}
