using Blazored.LocalStorage;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudExtensions;
using Syncfusion.Blazor.RichTextEditor;


namespace EzioLearning.Wasm.Pages.Course
{
    public partial class CourseCreate : IAsyncDisposable
    {
        [Inject] private IStringLocalizer<CourseCreate> Localizer { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;

        [Inject] private ICourseCategoryService CourseCategoryService { get; set; } = default!;
        [Inject] private ICourseService CourseService { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private ISnackBarService SnackBarService { get; set; } = default!;
        private ICollection<CourseCategoryViewDto> CourseCategories { get; set; } = new List<CourseCategoryViewDto>();
        private IEnumerable<CourseCategoryViewDto> SelectedCourseCategories { get; set; } = [];

        private RenderFragment? CourseCategoriesRenderFragment { get; set; }

        [SupplyParameterFromForm] private CourseCreateDto CourseCreateDto { get; set; } = new();

        private static readonly List<ToolbarItemModel> Tools =
        [
            new ToolbarItemModel() { Command = ToolbarCommand.FontName },
            new ToolbarItemModel() { Command = ToolbarCommand.FontSize },
            new ToolbarItemModel() { Command = ToolbarCommand.FontColor },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.Bold },
            new ToolbarItemModel() { Command = ToolbarCommand.Italic },
            new ToolbarItemModel() { Command = ToolbarCommand.Underline },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.Formats },
            new ToolbarItemModel() { Command = ToolbarCommand.Alignments },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.CreateLink },
            new ToolbarItemModel() { Command = ToolbarCommand.Image },
            new ToolbarItemModel() { Command = ToolbarCommand.Video },
            new ToolbarItemModel() { Command = ToolbarCommand.Audio },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.CreateTable },
            new ToolbarItemModel() { Command = ToolbarCommand.SourceCode },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },

            new ToolbarItemModel() { Command = ToolbarCommand.Undo },
            new ToolbarItemModel() { Command = ToolbarCommand.Redo },
            new ToolbarItemModel() { Command = ToolbarCommand.Separator },
        ];
        public string? ImagePreviewUrl { get; set; }

        private int ActiveTab { get; set; }

        protected override async Task OnInitializedAsync()
        {
          
            CourseCategories = (await CourseCategoryService.GetCourseCategories());

            CourseCategoriesRenderFragment = RecursiveSelect(CourseCategories.ToList());


            await RestoreModel();

        }


        private async Task RestoreModel()
        {
            var courseCreateDto = (await LocalStorageService.GetItemAsync<CourseCreateDto>(nameof(CourseCreateDto)));
            CourseCreateDto = courseCreateDto ?? new();

            SelectedCourseCategories = CourseCategories
                .Where(x => CourseCreateDto.CourseCategoryIds.Contains(x.Id))
                .ToArray();
        }


        private RenderFragment RecursiveSelect(List<CourseCategoryViewDto> categories, Guid? parentId = null)
        {
            return builder =>
            {
                var childCategories = categories.Where(c => c.ParentId == parentId).ToList();

                if (!childCategories.Any()) return;

                foreach (var childCategory in childCategories)
                {
                    var paddingText = $"{childCategory.Name}";
                    var subChildCategories = categories
                        .Where(x => x.ParentId == childCategory.Id).ToList();

                    if (subChildCategories.Any())
                    {
                        builder.OpenComponent<MudSelectItemGroupExtended<CourseCategoryViewDto>>(0);
                        builder.AddAttribute(1, "T", typeof(CourseCategoryViewDto));
                        builder.AddAttribute(2, "Text", paddingText);
                        builder.AddAttribute(3, "Nested", true);
                        builder.AddAttribute(4, "Sticky", true);
                        builder.AddAttribute(5, "InitiallyExpanded", false);
                        builder.AddAttribute(6, "ChildContent", RecursiveSelect(categories, childCategory.Id));
                        builder.CloseComponent();
                    }
                    else
                    {
                        builder.OpenComponent<MudSelectItemExtended<CourseCategoryViewDto>>(0);
                        builder.AddAttribute(1, "T", typeof(CourseCategoryViewDto));
                        builder.AddAttribute(2, "Text", paddingText);
                        builder.AddAttribute(3, "Value", childCategory);
                        builder.CloseComponent();
                    }

                }

            };
        }


        private void ResetCourseCategories()
        {
            SelectedCourseCategories = [];
            CourseCreateDto.CourseCategoryIds = [];
            StateHasChanged();
        }

        private async Task CreateNewCourseSubmit()
        {
            CourseCreateDto.CourseCategoryIds = SelectedCourseCategories.Select(x => x.Id).ToArray();
            
            var response = await CourseService.CreateNewCourse(CourseCreateDto);
            if (response.IsSuccess)
            {
                await LocalStorageService.RemoveItemAsync(nameof(CourseCreateDto));
                Snackbar.Add(response.Message, Severity.Success);
                ActiveTab = 1;
            }
            else
            {
                SnackBarService.ShowErrorFromResponse(response);
            }
        }



        private async Task LoadFile(InputFileChangeEventArgs file)
        {
            if (!FileConstants.AcceptTypes.Contains(Path.GetExtension(file.File.Name))) return;

            var tempFile = file.File;

            var buffer = new byte[tempFile.Size];

            var stream = tempFile.OpenReadStream(tempFile.Size);
            var readAsync = await stream.ReadAsync(buffer);

            if (readAsync <= 0) return;

            var base64 = Convert.ToBase64String(buffer);

            var mimeType = tempFile.ContentType;
            ImagePreviewUrl = $"data:{mimeType};base64,{base64}";
            CourseCreateDto.PosterImage = tempFile;

            StateHasChanged();
        }
        public async ValueTask DisposeAsync()
        {
            CourseCreateDto.CourseCategoryIds = SelectedCourseCategories.Select(x => x.Id).ToArray();

            await LocalStorageService.SetItemAsync(nameof(CourseCreateDto), CourseCreateDto);
        }
    }
}