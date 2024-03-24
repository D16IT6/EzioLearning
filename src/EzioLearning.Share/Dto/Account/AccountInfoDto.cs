﻿namespace EzioLearning.Share.Dto.Account
{
    public class AccountInfoDto
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? FullName => FirstName + " " + LastName;
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; } = DateTime.UtcNow.AddYears(-10);

        public string[] Roles { get; set; } = [];

        public string? Avatar { get; set; }


    }
}