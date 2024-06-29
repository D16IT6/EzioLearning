using Blazored.LocalStorage;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Utils;
using EzioLearning.Share.Validators.Common;
using EzioLearning.Wasm.Dto.Learning.Course;
using EzioLearning.Wasm.Services.Interface;
using EzioLearning.Wasm.Utils.Common;
using EzioLearning.Wasm.Utils.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using MudBlazor;
using MudExtensions;
using Syncfusion.Blazor.RichTextEditor;

namespace EzioLearning.Wasm.Pages.Account.Course
{
    public partial class CourseUpdate : AccountComponentBase
    {
        [Parameter] public Guid CourseId { get; set; }
        private CourseUpdateBlazorDto CourseDto { get; set; } = new();

        [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

        [Inject] private IStringLocalizer<CourseCreate> Localizer { get; set; } = default!;
        [Inject] private ILocalStorageService LocalStorageService { get; set; } = default!;
        [Inject] private ICourseCategoryService CourseCategoryService { get; set; } = default!;
        [Inject] private ICourseService CourseService { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        private ICollection<CourseCategoryViewDto> CourseCategories { get; set; } = new List<CourseCategoryViewDto>();
        private IEnumerable<CourseCategoryViewDto> SelectedCourseCategories { get; set; } = new List<CourseCategoryViewDto>();

        private RenderFragment? CourseCategoriesRenderFragment { get; set; }

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
            new ToolbarItemModel() { Command = ToolbarCommand.OrderedList },
            new ToolbarItemModel() { Command = ToolbarCommand.UnorderedList },
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

        private CourseSectionUpdateBlazorDto CourseSectionCreateDto { get; set; } = new();

        public string? ImagePreviewUrl { get; set; }


        public string? DefaultPosterUrl { get; set; } = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var response = await CourseService.GetCourseUpdate(CourseId);

            CourseDto = response.Data!;

            ImagePreviewUrl = CourseDto.Poster;
            DefaultPosterUrl = CourseDto.Poster;

            CourseCategories = (await CourseCategoryService.GetCourseCategories());

            var selectedCourseCategoryIds = CourseDto.CourseCategories.Select(c => c.Id).ToList();

            SelectedCourseCategories = CourseCategories.Where(x => selectedCourseCategoryIds.Contains(x.Id)).ToList();

            CourseCategoriesRenderFragment = RecursiveSelect(CourseCategories.ToList());

            foreach (var lecture in CourseDto.Sections.SelectMany(x => x.Lectures))
            {
                lecture.TempFileUrl = lecture.FileUrl;
            }
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


        private async Task OnCoursePosterChanged(CourseUpdateBlazorDto courseDto, InputFileChangeEventArgs inputFileChangeEventArgs)
        {
            var file = inputFileChangeEventArgs.File;
            courseDto.PosterImage = file;
            ImagePreviewUrl = await file.GetBlobStream(JsRuntime);
        }

        private void OnDeleteImageClick(MouseEventArgs obj)
        {
            ImagePreviewUrl = DefaultPosterUrl;
            CourseDto.PosterImage = null;
        }

        private void CourseLessonUpdate(MudItemDropInfo<CourseLectureUpdateBlazorDto> dropItem)
        {
            var draggedItem = dropItem.Item;

            if (draggedItem == null) return;

            var courseSection = CourseDto.Sections.FirstOrDefault(x => x.Id == draggedItem.CourseSectionId);
            if (courseSection == null) return;

            var dropContainer = courseSection.CourseLectureDropContainer;

            if (!dropContainer.HasTransactionIndexChanged()) return;

            var targetOrder = dropItem.IndexInZone;
            var draggedItemOriginalOrder = draggedItem.SortOrder;

            var lesson = courseSection.Lectures.First(lesson => lesson.SortOrder == targetOrder);

            lesson.SortOrder = draggedItemOriginalOrder;

            draggedItem.SortOrder = targetOrder;

            courseSection.Lectures = courseSection.Lectures.OrderBy(x => x.SortOrder).ToList();

            Console.WriteLine(string.Join(',', courseSection.Lectures.Select(x => $"{x.Name}")));


#pragma warning disable BL0005
            dropContainer.Items = courseSection.Lectures;
#pragma warning restore BL0005
            dropContainer.Refresh();
        }

        private Task CreateNewLecture(CourseSectionUpdateBlazorDto courseSectionDto)
        {
            var newLecture = new CourseLectureUpdateBlazorDto()
            {
                Name = $"Course Lecture {Random.Shared.Next(1000)}",
                CourseSectionId = courseSectionDto.Id,
                SortOrder = courseSectionDto.Lectures.Count()
            };

            courseSectionDto.Lectures.Add(newLecture);
            UpdateLectureType(newLecture, newLecture.LectureType);

            return Task.CompletedTask;
        }

        private Task DeleteSection(CourseSectionUpdateBlazorDto courseSectionDto)
        {
            courseSectionDto.Deleted = true;

            //StateHasChanged();

            return Task.CompletedTask;
        }

        private Task UpdateLectureType(CourseLectureUpdateBlazorDto dropContext, CourseLectureType courseLectureType)
        {
            dropContext.LectureType = courseLectureType;
            dropContext.FileUpload = null;
            dropContext.TempFileUrl = string.Empty;
            return Task.CompletedTask;
        }

        private async Task OnCourseLectureFileChanged(CourseLectureUpdateBlazorDto lectureUpdate, InputFileChangeEventArgs inputFile)
        {
            try
            {
                var currentAcceptType = lectureUpdate.FileUploadContainer.Accept;

                var acceptTypes = new List<string>();
                if (!string.IsNullOrWhiteSpace(currentAcceptType))
                {
                    acceptTypes = currentAcceptType.Split(',').ToList();
                }
                else
                {
                    acceptTypes.AddRange(lectureUpdate.LectureType == CourseLectureType.Video
                        ? FileConstants.VideoAcceptTypes
                        : FileConstants.DocumentAcceptTypes);
                }

                var file = inputFile.File;
                var maxFileSize = lectureUpdate.LectureType == CourseLectureType.Video
                    ? FileConstants.VideoUploadLimit
                    : FileConstants.DocumentUploadLimit;

                var fileExtension = Path.GetExtension(file.Name);
                if (file.Size >= maxFileSize || !acceptTypes.Contains(fileExtension))
                {
                    Snackbar.Add(
                        $"Vui lòng chọn file đúng loại định dạng:<br>" +
                        $"Video: {string.Join(',', FileConstants.VideoAcceptTypes)}, " +
                        $"<= {FileConstants.VideoUploadLimit.ShowFormatFileSize()}<br>" +
                        $"Document: {string.Join(',', FileConstants.DocumentAcceptTypes)}, " +
                        $"<= {FileConstants.DocumentUploadLimit.ShowFormatFileSize()}", Severity.Error
                    );

                    lectureUpdate.FileUpload = null;
                    lectureUpdate.TempFileUrl = lectureUpdate.FileUrl;
                    return;
                }

                lectureUpdate.TempFileUrl = await file.GetBlobStream(JsRuntime);
            }
            catch (Exception)
            {
                Snackbar.Add("file quá nặng, không thể mở preview", Severity.Error);
            }
        }

        private Task DeleteLecture(CourseSectionUpdateBlazorDto _, CourseLectureUpdateBlazorDto dropContext)
        {
            dropContext.Deleted = true;

            return Task.CompletedTask;
        }

        private async Task SubmitUpdateCourse()
        {
            CourseDto.CourseCategories.Clear();

            CourseDto.CourseCategories.AddRange(SelectedCourseCategories);

            var response = await CourseService.UpdateCourse(CourseDto);
            Snackbar.Add(response.Message, response.IsSuccess ? Severity.Success : Severity.Error);

            if (response.IsSuccess)
            {
                await NavigationService.Navigate(RouteConstants.CourseRoute.CourseIndex, "", 1);
            }

        }

        private void CreateNewSection()
        {
            CourseDto.Sections.Add(CourseSectionCreateDto);

        }
    }
}
