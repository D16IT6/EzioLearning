using AutoMapper;
using EzioLearning.Core.Dto.Auth;
using EzioLearning.Core.Dto.Learning.Course;
using EzioLearning.Domain.Entities.Identity;
using EzioLearning.Domain.Entities.Learning;
using EzioLearning.Share.Dto.Account;
using EzioLearning.Share.Dto.Learning.Course;
using EzioLearning.Share.Dto.Learning.CourseCategory;
using EzioLearning.Share.Dto.User;

namespace EzioLearning.Core.Dto;

public class MapperClass
{
    #region Learning

    public class CourseCreateApiDtoProfile : Profile
    {
        public CourseCreateApiDtoProfile()
        {
            CreateMap<CourseCreateApiDto, Domain.Entities.Learning.Course>();
        }
    }

    public class CourseCategoryViewDtoProfile : Profile
    {
        public CourseCategoryViewDtoProfile()
        {
            CreateMap<CourseCategoryViewDto, Domain.Entities.Learning.CourseCategory>();

            CreateMap<Domain.Entities.Learning.CourseCategory, CourseCategoryViewDto>()
                .ForMember(dest => dest.ParentName,
                    otp =>
                        otp.MapFrom(src => src.Parent!.Name));
        }
    }

    public class TopCourseCategoryDtoProfile : Profile
    {
        public TopCourseCategoryDtoProfile()
        {
            CreateMap<TopCourseCategoryDtoProfile, Domain.Entities.Learning.CourseCategory>();
            CreateMap<Domain.Entities.Learning.CourseCategory, TopCourseCategoryDtoProfile>();
        }
    }

    public class CourseCategoryCreateDtoProfile : Profile
    {
        public CourseCategoryCreateDtoProfile()
        {
            CreateMap<CourseCategoryCreateDto, CourseCategory>();
            CreateMap<CourseCategory, CourseCategoryCreateDto>();
        }
    }

    public class CourseCreateDtoProfile : Profile
    {
        public CourseCreateDtoProfile()
        {
            CreateMap<CourseCreateDto, Course>();
        }
    }
    public class InstructorViewDtoProfile : Profile
    {
        public InstructorViewDtoProfile()
        {
            CreateMap<AppUser, InstructorViewDto>()
                .ForMember(x => x.Name, cfg => cfg.MapFrom(x => $"{x.FirstName} {x.LastName}"))
                .ForMember(x => x.StudentCount, cfg => cfg.MapFrom(x => x.Students.Count));
        }
    }

    #endregion

    #region User

    public class UserCreateDtoProfile : Profile
    {
        public UserCreateDtoProfile()
        {
            CreateMap<UserCreateApiDto, AppUser>()
                .ForMember(x => x.Avatar, cfg => cfg.Ignore());
            CreateMap<AppUser, UserCreateApiDto>()
                .ForMember(x => x.Avatar, cfg => cfg.Ignore());
        }
    }


    #endregion

    #region Auth

    public class RegisterRequestDtoProfile : Profile
    {
        public RegisterRequestDtoProfile()
        {
            CreateMap<RegisterRequestApiDto, AppUser>()
                .ForMember(x => x.Avatar, cfg => cfg.Ignore());
            CreateMap<AppUser, RegisterRequestApiDto>()
                .ForMember(x => x.Avatar, cfg => cfg.Ignore());
        }
    }

    #endregion

    #region Account

    public class AccountInfoMinimalDtoProfile : Profile
    {
        public AccountInfoMinimalDtoProfile()
        {
            CreateMap<AppUser, AccountInfoMinimalDto>();
        }
    }

    public class AccountInfoDtoProfile : Profile
    {
        public AccountInfoDtoProfile()
        {
            CreateMap<AppUser, AccountInfoDto>().ForMember(x => x.FullName, opt => opt.Ignore());
        }
    }

    #endregion
}
