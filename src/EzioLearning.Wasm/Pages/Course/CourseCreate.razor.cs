using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Wasm.Services.Interface;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudExtensions;


namespace EzioLearning.Wasm.Pages.Course
{
	public partial class CourseCreate
	{
		[Inject] private IStringLocalizer<CourseCreate> Localizer { get; set; } = default!;

		[Inject] private ICourseCategoryService CourseCategoryService { get; set; } = default!;
        private ICollection<CourseCategoryViewDto> CourseCategories { get; set; } = new List<CourseCategoryViewDto>();
        private IEnumerable<CourseCategoryViewDto> SelectedCourseCategories { get; set; } = [];
        private CourseCategoryViewDto SelectedCourseCategory { get; set; } = new();

        private RenderFragment? CourseCategoriesRenderFragment { get; set; }

		protected override async Task OnInitializedAsync()
		{
			CourseCategories = (await CourseCategoryService.GetCourseCategories());
            
            CourseCategoriesRenderFragment = RecursiveSelect(CourseCategories.ToList());
        }

        List<CourseCategoryViewDto> FlattenCourseCategories(List<CourseCategoryViewDto> categories, Guid? parentId = null)
        {
            var flattenedCategories = new List<CourseCategoryViewDto>();

            foreach (var category in categories.Where(x => x.ParentId == parentId))
            {
                flattenedCategories.Add(category);

                var subCategories = FlattenCourseCategories(categories, category.Id);

                flattenedCategories.AddRange(subCategories);
            }
            return flattenedCategories;
        }
        protected RenderFragment RecursiveSelect(List<CourseCategoryViewDto> categories, Guid? parentId = null)
        {
            return builder =>
            {
                var childCategories = categories.Where(c => c.ParentId == parentId).ToList();

                if (!childCategories.Any()) return;

                foreach (var childCategory in childCategories)
                {
                    var subChildCategories = categories
                        .Where(x => x.ParentId == childCategory.Id).ToList();

                    if (subChildCategories.Any())
                    {
                        builder.OpenComponent<MudSelectItemGroupExtended<CourseCategoryViewDto>>(0);
                        builder.AddAttribute(1, "T", typeof(CourseCategoryViewDto));
                        builder.AddAttribute(2, "Text", childCategory.Name);
                        builder.AddAttribute(3,"Nested",true);
                        builder.AddAttribute(4, "Sticky", true);
                        builder.AddAttribute(5, "InitiallyExpanded", true);
                        builder.AddAttribute(6, "ChildContent", RecursiveSelect(categories, childCategory.Id));
                        builder.CloseComponent();

                    }
                    else
                    {
                        builder.OpenComponent<MudSelectItemExtended<CourseCategoryViewDto>>(0);
                        builder.AddAttribute(1, "T", typeof(CourseCategoryViewDto));
                        builder.AddAttribute(2, "Text", childCategory.Name);
                        builder.AddAttribute(3, "Value", childCategory);
                        builder.CloseComponent();

                    }

                }

            };
        }

        private bool SearchCourseCategories(CourseCategoryViewDto value, string inputText)
        {
            return value.Name!.ToLower().Contains(inputText.ToLower());
        }
    }
}
