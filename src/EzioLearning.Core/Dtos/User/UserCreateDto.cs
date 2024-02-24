using AutoMapper;
using EzioLearning.Core.Dtos.Auth;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dtos.User
{
    public class UserCreateDto : RegisterRequestDto
    {
        public Guid[] RoleIds { get; set; } = [];

        public class UserCreateDtoProfile : Profile
        {
            public UserCreateDtoProfile()
            {
                CreateMap<UserCreateDto, AppUser>()
                    .ForMember(x => x.Avatar, cfg => cfg.Ignore());
                CreateMap<AppUser, UserCreateDto>()
                    .ForMember(x => x.Avatar, cfg => cfg.Ignore());

            }
        }

    }
}
