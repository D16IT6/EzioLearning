using System.Net;
using System.Security.Claims;
using AutoMapper;
using EzioLearning.Api.Filters;
using EzioLearning.Api.Services;
using EzioLearning.Api.Utils;
using EzioLearning.Core.Dto.Learning.Course;
using EzioLearning.Core.Repositories.Learning;
using EzioLearning.Core.Repositories.Resources;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Domain.Entities.Resources;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.User;
using EzioLearning.Share.Models.Pages;
using EzioLearning.Share.Models.Request;
using EzioLearning.Share.Models.Response;
using EzioLearning.Share.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static EzioLearning.Share.Common.Permissions;

namespace EzioLearning.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CourseController(
    ICourseRepository courseRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    FileService fileService,
    VideoService videoService,
    IStudentRepository studentRepository,
    ICourseSectionRepository courseSectionRepository,
    ICourseLectureRepository courseLectureRepository,
    ICourseCategoryRepository courseCategoryRepository,
    IVideoRepository videoRepository,
    IDocumentRepository documentRepository,
    UserManager<AppUser> userManager, IStringLocalizer<CourseController> localizer) : ControllerBase
{
    private static readonly string ImageFolderPath = "Uploads/Images/Courses/";
    private static readonly string VideoFolderPath = "Uploads/Videos/Courses/";
    private static readonly string DocumentFolderPath = "Uploads/Documents/Courses/";


    [HttpGet]
    public async Task<IActionResult> GetCourse([FromQuery] CourseListOptions options)
    {
        var pagedResult = await courseRepository.GetPageWithDto<CourseItemViewDto>(
            expression: c => ((c.Price > 0 && options.PriceType == PriceType.Paid)
                            || (c.Price >= 0 && options.PriceType == PriceType.All)
                            || (c.Price == 0 && options.PriceType == PriceType.Free))
                            && (options.SearchText == null || c.Name.Contains(options.SearchText))
                            && (!options.CourseCategoryIds.Any() ||
                            options.CourseCategoryIds.Except(c.Categories.Select(x => x.Id).ToList()).Count() < options.CourseCategoryIds.Count)
            ,
            pageNumber: options.PageNumber,
            pageSize: options.PageSize);


        return Ok(new ResponseBaseWithData<PageResult<CourseItemViewDto>>()
        {
            Data = pagedResult,
            Message = "Lấy danh sách thành công"
        });
    }

    [HttpGet("Purchased")]
    [Authorize]
    public async Task<IActionResult> GetPagePurchasedCourses([FromQuery] CourseListOptions options)
    {
        var userId = Guid.Parse(User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value);

        var pagedResult = await courseRepository.GetPageWithDto<CoursePurchasedItemViewDto>(
            expression: c =>  (options.SearchText == null || c.Name.Contains(options.SearchText))
                              && (!options.CourseCategoryIds.Any() ||
                                  options.CourseCategoryIds.Except(c.Categories.Select(x => x.Id).ToList()).Count() < options.CourseCategoryIds.Count)
                              && c.Students.Where(x=> x.Confirm && x.UserId == userId).Select(x=>x.UserId).Contains(userId),
            pageNumber: options.PageNumber,
            pageSize: options.PageSize,
            includes: [nameof(Course.Students)]
            );

        return Ok(new ResponseBaseWithData<PageResult<CoursePurchasedItemViewDto>>()
        {
            Data = pagedResult,
            Message = "Lấy danh sách thành công"
        });
    }


    [HttpGet("Teacher/{teacherId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetCoursePageByTeacherId([FromRoute] Guid teacherId, [FromQuery] CourseListOptions options)
    {
        var pagedResult = await courseRepository.GetPageWithDto<CourseItemViewDto>(
            expression: c => ((c.Price > 0 && options.PriceType == PriceType.Paid)
                              || (c.Price >= 0 && options.PriceType == PriceType.All)
                              || (c.Price == 0 && options.PriceType == PriceType.Free))
                             && (options.SearchText == null || c.Name.Contains(options.SearchText))
                             && (!options.CourseCategoryIds.Any() ||
                                 options.CourseCategoryIds.Except(c.Categories.Select(x => x.Id).ToList()).Count() < options.CourseCategoryIds.Count)
                             && teacherId == c.CreatedBy,
            pageNumber: options.PageNumber,
            pageSize: options.PageSize);

        return Ok(new ResponseBaseWithData<PageResult<CourseItemViewDto>>()
        {
            Data = pagedResult,
            Message = "Lấy danh sách thành công"
        });
    }

    [HttpGet("Update/{courseId:guid}")]
    [VerifyToken]
    [Authorize(Courses.Edit)]
    public async Task<IActionResult> GetCourseUpdateInfo([FromRoute] Guid courseId)
    {
        var teacherId = Guid.Parse(User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value);

        var course = await courseRepository.GetCourseUpdate(courseId, teacherId);
        if (course == null)
            return NotFound(new ResponseBase()
            {
                Errors = new Dictionary<string, string[]>()
                {
                    { "NotFound", ["Không tìm thấy"] }
                },
                Status = HttpStatusCode.NotFound
            });

        var responseData = mapper.Map<CourseUpdateDto>(course);

        return Ok(new ResponseBaseWithData<CourseUpdateDto>()
        {
            Data = responseData,
            Status = HttpStatusCode.OK
        });
    }

    [HttpPut]
    [VerifyToken]
    [Authorize(Courses.Edit)]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> UpdateCourseInfo([FromForm] CourseUpdateApiDto courseUpdate)
    {
        var course = await courseRepository.FindCourseUpdate(courseUpdate.Id);

        if (course == null)
        {
            return NotFound(new ResponseBase()
            {
                Message = "Không tìm thấy khoá học",
                Status = HttpStatusCode.NotFound
            });
        }

        course.Content = courseUpdate.Content;
        course.Description = courseUpdate.Description;
        course.Level = courseUpdate.Level;
        course.Price = courseUpdate.Price;
        course.Name = courseUpdate.Name;

        var poster = courseUpdate.NewPoster;
        if (poster is { Length: > 0 })
        {
            if (!fileService.IsImageAccept(poster.FileName))
                return BadRequest(new ResponseBase
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = localizer.GetString("ImageExtensionNotAllow")
                });

            course.Poster = await fileService.SaveFile(poster, ImageFolderPath, course.Id.ToString());
        }
        #region Update Course Categories

        var courseCategories = course.Categories.ToList(); // Ensure we're working with a concrete list
        var courseCategoryIds = courseCategories.Select(x => x.Id).ToList();

        var allCourseCategories = (await courseCategoryRepository.GetAllAsync()).ToList();

        var newCourseCategoryIds = courseUpdate.CourseCategories.Select(c => c.Id).ToList();

        // Find categories to be deleted
        var deletedCourseCategories = courseCategories
            .Where(x => !newCourseCategoryIds.Contains(x.Id))
            .ToList(); // Convert to list to avoid modifying collection while iterating

        // Remove the deleted categories
        foreach (var deletedCourseCategory in deletedCourseCategories)
        {
            course.Categories.Remove(deletedCourseCategory);
        }

        // Find categories to be inserted
        var insertCourseCategoryIds = newCourseCategoryIds.Except(courseCategoryIds).ToList();

        foreach (var insertCourseCategoryId in insertCourseCategoryIds)
        {
            var insertCourseCategory = allCourseCategories.FirstOrDefault(x => x.Id == insertCourseCategoryId);
            if (insertCourseCategory != null)
            {
                course.Categories.Add(insertCourseCategory);
            }
        }

        #endregion


        #region Update Course Sections

        foreach (var sectionUpdateApiDto in courseUpdate.Sections)
        {
            if (sectionUpdateApiDto.Id == Guid.Empty)
            {
                sectionUpdateApiDto.Id = Guid.NewGuid();
                var newSection = mapper.Map<CourseSection>(sectionUpdateApiDto);

                foreach (var lecture in sectionUpdateApiDto.Lectures)
                {
                    lecture.Id = Guid.NewGuid();

                    var file = lecture.File;

                    var newLecture = mapper.Map<CourseLecture>(lecture);

                    var saveFolder = lecture.LectureType == CourseLectureType.Document ? DocumentFolderPath : VideoFolderPath;
                    saveFolder = Path.Combine(saveFolder, course.CreatedBy.ToString()!);

                    if (file == null) throw new FileNotFoundException("Cần có file để tạo bài học");
                    //Relative path
                    var outputFilePath = await fileService.SaveFile(file, saveFolder, Guid.NewGuid().ToString());


                    if (lecture.LectureType == CourseLectureType.Video)
                    {
                        var video = new Video()
                        {
                            Id = Guid.NewGuid(),
                            DefaultPath = outputFilePath,
                            Duration = await videoService.GetDurationFromVideo(outputFilePath),
                            Name = Path.GetFileName(outputFilePath),
                            Status = VideoStatus.Pending,
                        };
                        newLecture.LectureType = CourseLectureType.Video;
                        newLecture.Video = video;
                    }
                    else
                    {
                        var document = new Document()
                        {
                            Id = Guid.NewGuid(),
                            Name = Path.GetFileName(file.FileName),
                            Path = outputFilePath,
                        };
                        newLecture.LectureType = CourseLectureType.Document;
                        newLecture.Document = document;
                    }

                    courseLectureRepository.Add(newLecture);
                    newSection.CourseLectures.Add(newLecture);
                }
                courseSectionRepository.Add(newSection);
                course.Sections.Add(newSection);

            }

            else if (sectionUpdateApiDto.Deleted)
            {
                var deletedSection = course.Sections.First(x => x.Id == sectionUpdateApiDto.Id);
                courseSectionRepository.Remove(deletedSection);
                //course.Sections.Remove(deletedSection);

            }
            else
            {
                var courseSection = course.Sections.First(x => x.Id == sectionUpdateApiDto.Id);
                courseSection.Name = sectionUpdateApiDto.Name;

                foreach (var lectureUpdateApiDto in sectionUpdateApiDto.Lectures)
                {
                    if (lectureUpdateApiDto.Deleted)
                    {
                        var deletedLecture = course.Sections.SelectMany(x => x.CourseLectures)
                            .FirstOrDefault(x => x.Id == lectureUpdateApiDto.Id);
                        if (deletedLecture != null)
                        {
                            courseLectureRepository.Remove(deletedLecture);
                        }
                    }

                    else if (lectureUpdateApiDto.Id == Guid.Empty)
                    {
                        lectureUpdateApiDto.Id = Guid.NewGuid();

                        var file = lectureUpdateApiDto.File;

                        var newLecture = mapper.Map<CourseLecture>(lectureUpdateApiDto);

                        var saveFolder = lectureUpdateApiDto.LectureType == CourseLectureType.Document ? DocumentFolderPath : VideoFolderPath;
                        saveFolder = Path.Combine(saveFolder, course.CreatedBy.ToString()!);

                        if (file == null) throw new FileNotFoundException("Cần có file để tạo bài học");

                        //Relative path
                        var outputFilePath = await fileService.SaveFile(file, saveFolder, Guid.NewGuid().ToString());

                        if (lectureUpdateApiDto.LectureType == CourseLectureType.Video)
                        {
                            var video = new Video()
                            {
                                Id = Guid.NewGuid(),
                                DefaultPath = outputFilePath,
                                Duration = await videoService.GetDurationFromVideo(outputFilePath),
                                Name = Path.GetFileName(outputFilePath),
                                Status = VideoStatus.Pending,
                            };
                            videoRepository.Add(video);

                            newLecture.LectureType = CourseLectureType.Video;
                            newLecture.Video = video;

                        }
                        else
                        {
                            var document = new Document()
                            {
                                Id = Guid.NewGuid(),
                                Name = Path.GetFileName(file.FileName),
                                Path = outputFilePath,
                            };
                            documentRepository.Add(document);

                            newLecture.LectureType = CourseLectureType.Document;
                            newLecture.Document = document;
                        }
                        var sectionToInsert = course.Sections.FirstOrDefault(x => x.Id == lectureUpdateApiDto.CourseSectionId);
                        if (sectionToInsert != null)
                        {
                            courseLectureRepository.Add(newLecture);
                            sectionToInsert.CourseLectures.Add(newLecture);
                        }
                    }
                    else
                    {
                        var courseLecture = course.Sections.SelectMany(x => x.CourseLectures).FirstOrDefault(x => x.Id == lectureUpdateApiDto.Id);
                        if (courseLecture == null) continue;


                        courseLecture.Name = lectureUpdateApiDto.Name;
                        courseLecture.SortOrder = lectureUpdateApiDto.SortOrder;
                        if (lectureUpdateApiDto.File == null) continue;
                        courseLecture.LectureType = lectureUpdateApiDto.LectureType;

                        var saveFolder = lectureUpdateApiDto.LectureType == CourseLectureType.Document ? DocumentFolderPath : VideoFolderPath;
                        saveFolder = Path.Combine(saveFolder, course.CreatedBy.ToString()!);

                        var file = lectureUpdateApiDto.File;

                        if (file == null || file.Length <= 0) continue;
                        //Relative path
                        var outputFilePath = await fileService.SaveFile(file, saveFolder, Guid.NewGuid().ToString());

                        if (lectureUpdateApiDto.LectureType == CourseLectureType.Video)
                        {
                            var video = new Video()
                            {
                                Id = Guid.NewGuid(),
                                DefaultPath = outputFilePath,
                                Duration = await videoService.GetDurationFromVideo(outputFilePath),
                                Name = Path.GetFileName(outputFilePath),
                                Status = VideoStatus.Pending,
                            };
                            if (courseLecture.Video != null)
                            {
                                videoRepository.Remove(courseLecture.Video);
                            }

                            videoRepository.Add(video);

                            courseLecture.LectureType = CourseLectureType.Video;
                            courseLecture.Video = video;
                        }
                        else
                        {
                            var document = new Document()
                            {
                                Id = Guid.NewGuid(),
                                Name = Path.GetFileName(file.FileName),
                                Path = outputFilePath,
                            };
                            if (courseLecture.Document != null)
                            {
                                documentRepository.Remove(courseLecture.Document);
                            }
                            documentRepository.Add(document);

                            courseLecture.LectureType = CourseLectureType.Document;
                            courseLecture.Document = document;
                        }

                    }
                }
            }
        }

        #endregion

        await unitOfWork.CompleteAsync();
        return Ok(new ResponseBase()
        {
            Message = "Cập nhật thành công",
            Status = HttpStatusCode.Accepted
        });
    }

    [HttpGet("Detail/{courseId:guid}")]
    public async Task<IActionResult> GetCourseDetail([FromRoute] Guid courseId)
    {
        Guid.TryParse(User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.PrimarySid))?.Value,out var userId);
        var course = await courseRepository.GetCourseDetail(courseId);

        var isPurchased =( await studentRepository.Find(x => x.Confirm && x.UserId == userId && x.CourseId == courseId)).Any();
        var courseDetailViewDto = mapper.Map<CourseDetailViewDto>(course);
        courseDetailViewDto.Sections = courseDetailViewDto.Sections.OrderBy(x => x.Name).ToList();

        if (!isPurchased)
        {
            foreach (var section in courseDetailViewDto.Sections)
            {
                var hasFirstVideoUrlBeenKept = false;

                foreach (var lecture in section.Lectures)
                {
                    if (lecture.LectureType == CourseLectureType.Video && !hasFirstVideoUrlBeenKept)
                    {
                        hasFirstVideoUrlBeenKept = true;
                    }
                    else
                    {
                        lecture.FileUrl = string.Empty;
                    }
                }
            }
        }

        courseDetailViewDto.Purchased = isPurchased;

        return Ok(new ResponseBaseWithData<CourseDetailViewDto>()
        {
            Data = courseDetailViewDto,
            Message = "Lấy thông tin khoá học thành công"
        });
    }

    [HttpGet("Count")]
    public async Task<IActionResult> CountCourse()
    {
        var count = await courseRepository.CountCourses();
        return Ok(new ResponseBaseWithData<int>
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("CourseCountSuccess"),
            Data = count
        });
    }


    [HttpGet("FeaturedInstructor/{take:int?}")]
    public Task<IActionResult> GetFeatureInstructors([FromRoute] int take = 12)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(take);

        var userList = userManager.Users
            .OrderByDescending(x => x.Students.Count)
            .ThenByDescending(x => x.Courses.Count)
            .Take(take);
        return Task.FromResult<IActionResult>(Ok(new ResponseBaseWithList<InstructorViewDto>
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("FeatureInstructorsGetSuccess"),
            Data = mapper.ProjectTo<InstructorViewDto>(userList)
        }));
    }


    [HttpGet("Feature/{take:int?}")]
    public async Task<IActionResult> GetFeaturedCourses([FromRoute] int take = 12)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(take);
        var data = (await courseRepository.GetFeaturedCourses(take)).AsQueryable();

        return Ok(new ResponseBaseWithList<CourseViewDto>
        {
            Status = HttpStatusCode.OK,
            Message = localizer.GetString("CourseFeatureGetSuccess"),
            Data = mapper.ProjectTo<CourseViewDto>(data)
        });
    }

    [HttpPost]
    [Authorize(Courses.Create)]
    [VerifyToken]
    public async Task<IActionResult> CreateNewCourse([FromForm] CourseCreateApiDto model)
    {
        var userId = User.Claims.First(x => x.Type.Equals(ClaimTypes.PrimarySid)).Value;

        model.CreatedBy = Guid.Parse(userId);
        model.Id = Guid.NewGuid();

        var newCourse = mapper.Map<Course>(model);

        var imagePath = ImageConstants.DefaultCoursePoster;

        var poster = model.Poster;
        if (poster is { Length: > 0 })
        {
            if (!fileService.IsImageAccept(poster.FileName))
                return BadRequest(new ResponseBase
                {
                    Status = HttpStatusCode.BadRequest,
                    Message = localizer.GetString("ImageExtensionNotAllow")
                });

            imagePath = await fileService.SaveFile(poster, ImageFolderPath, newCourse.Id.ToString());
        }

        newCourse.Poster = imagePath;

        newCourse.Categories = await GetInsertCourseCategories(model);

        courseRepository.Add(newCourse);

        var result = await unitOfWork.CompleteAsync();

        if (result > 0)
            return Ok(new ResponseBaseWithData<CourseCreateDto>()
            {
                Message = localizer.GetString("CourseCreateSuccess"),
                Status = HttpStatusCode.OK,
                Data = model
            });

        return BadRequest(new ResponseBase
        {
            Message = localizer.GetString("CourseCreateFail"),
            Status = HttpStatusCode.BadRequest
        });
    }

    [HttpPost("Section")]
    [Authorize(Courses.Create)]
    [VerifyToken]
    [RequestSizeLimit(long.MaxValue)]
    public async Task<IActionResult> CreateNewCourseSection([FromForm] CourseSectionCreateApiDto courseSectionCreateDto)
    {
        var userId = User.Claims.First(x => x.Type.Equals(ClaimTypes.Sid)).Value;

        courseSectionCreateDto.Id = Guid.NewGuid();

        var courseSection = mapper.Map<CourseSection>(courseSectionCreateDto);

        courseSection.CourseLectures.Clear();

        foreach (var courseLectureCreateDto in courseSectionCreateDto.CourseLectures)
        {
            courseLectureCreateDto.Id = Guid.NewGuid();

            var courseLecture = mapper.Map<CourseLecture>(courseLectureCreateDto);

            var file = courseLectureCreateDto.File;


            var saveFolder = courseLectureCreateDto.LectureType == CourseLectureType.Document ? DocumentFolderPath : VideoFolderPath;
            saveFolder = Path.Combine(saveFolder, userId);

            //Relative path
            var outputFilePath = await fileService.SaveFile(file, saveFolder, Guid.NewGuid().ToString());


            if (courseLectureCreateDto.LectureType == CourseLectureType.Video)
            {
                var video = new Video()
                {
                    Id = Guid.NewGuid(),
                    DefaultPath = outputFilePath,
                    Duration = await videoService.GetDurationFromVideo(outputFilePath),
                    Name = Path.GetFileName(outputFilePath),
                    Status = VideoStatus.Pending,
                };
                courseLecture.LectureType = CourseLectureType.Video;
                courseLecture.Video = video;
            }
            else
            {
                var document = new Document()
                {
                    Id = Guid.NewGuid(),
                    Name = Path.GetFileName(file.FileName),
                    Path = outputFilePath,
                };
                courseLecture.LectureType = CourseLectureType.Document;

                courseLecture.Document = document;
            }

            courseSection.CourseLectures.Add(courseLecture);
        }
        var addNewSectionResult = await courseRepository.AddNewSection(courseSection.CourseId, courseSection);


        var unitOfWorkResult = addNewSectionResult == 1 ? await unitOfWork.CompleteAsync() : addNewSectionResult;

        if (unitOfWorkResult > 0)
        {
            return Ok(new ResponseBaseWithData<CourseSectionCreateDto>()
            {
                Data = courseSectionCreateDto,
                Message = "Tạo phần khoá học thành công",
                Status = HttpStatusCode.OK
            });

        }
        return BadRequest(new ResponseBase
        {
            Message = localizer.GetString("CourseSectionCreateFail"),
            Status = HttpStatusCode.BadRequest
        });
    }

    private async Task<ICollection<CourseCategory>> GetInsertCourseCategories(CourseCreateApiDto courseCreateDto)
    {

        var result =
            await courseCategoryRepository
                .Find(x =>
                    courseCreateDto.CourseCategoryIds.Contains(x.Id)
                    );
        return result.ToList();

    }
}