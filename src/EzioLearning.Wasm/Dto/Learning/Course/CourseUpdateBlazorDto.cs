using EzioLearning.Share.Dto.Learning.Course;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace EzioLearning.Wasm.Dto.Learning.Course
{
    public class CourseUpdateBlazorDto : CourseUpdateDto
    {
        public IBrowserFile? PosterImage { get; set; }
        public List<CourseSectionUpdateBlazorDto> Sections { get; set; } = [];
    }

    public class CourseSectionUpdateBlazorDto : CourseSectionUpdateDto
    {
        public new List<CourseLectureUpdateBlazorDto> Lectures { get; set; } = [];

        public Severity Severity { get; set; }

        public MudDropContainer<CourseLectureUpdateBlazorDto> CourseLectureDropContainer { get; set; } = new();
    }

    public class CourseLectureUpdateBlazorDto : CourseLectureUpdateDto
    {
        public string? TempFileUrl { get; set; }

        public IBrowserFile? FileUpload { get; set; }

        public MudFileUpload<IBrowserFile> FileUploadContainer { get; set; } = new();
    }
}
