using AutoMapper;
using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dtos.User
{
    public class UserCreateDto
    {
        public string? FirstName { get; set; } = "Vũ Thế";
        public string? LastName { get; set; } = "Mạnh";
        public string? UserName { get; set; } = "vuthemanh333";

        public string? Email { get; set; } = "vuthemanh333@hotmail.com";
        public required string Password { get; set; } = "manhngu123";
        public required string ConfirmPassword { get; set; } = "manhngu123";
        public string? PhoneNumber { get; set; } = "0987654321";

        public DateOnly DateOfBirth { get; set; } = new(2003, 07, 17);

        public IFormFile? Avatar { get; set; }

        public class UserCreateDtoProfile : Profile
        {
            public UserCreateDtoProfile()
            {
                CreateMap<UserCreateDto, AppUser>();
                CreateMap<AppUser, UserCreateDto>();
            }
        }

    }
}
