using EzioLearning.Core.Dto.Learning.Course;
using EzioLearning.Share.Utils;
using EzioLearning.Share.Validators.Common;
using EzioLearning.Share.Validators.Learning.Course;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace EzioLearning.Core.Validators.Course
{
    public class CourseLectureCreateApiDtoValidator : AbstractValidator<CourseLectureCreateApiDto>
    {
        public CourseLectureCreateApiDtoValidator(
            IStringLocalizer<CourseLectureCreateApiDtoValidator> localizer,
            CourseLectureCreateDtoValidator validation )
        {
            Include(validation);
            RuleFor(x => x.File)
                .NotNull().WithMessage("Vui lòng chọn file")
                .Must(file => file is { Length: > 0 }).WithMessage("File bị lỗi, vui lòng chọn lại");

            RuleFor(x => x.File)
                .Must(fileUpload =>
                {
                    if (fileUpload == null) return false;
                    var fileExtension = Path.GetExtension(fileUpload.Name);
                    return
                        FileConstants.VideoAcceptTypes.Contains(fileExtension) &&
                        fileUpload.Length <= FileConstants.VideoUploadLimit;
                })
                .When(x => x.LectureType == CourseLectureType.Video)
                .WithMessage("Dung lượng quá lớn hoặc sai định dạng.");

            RuleFor(x => x.File)
                .Must(fileUpload =>
                {
                    if (fileUpload == null) return false;
                    var fileExtension = Path.GetExtension(fileUpload.Name);
                    return
                        FileConstants.DocumentAcceptTypes.Contains(fileExtension) &&
                        fileUpload.Length <= FileConstants.DocumentUploadLimit;
                })
                .When(x => x.LectureType == CourseLectureType.Document)
                .WithMessage("Dung lượng quá lớn hoặc sai định dạng.");
        }
    }
}
