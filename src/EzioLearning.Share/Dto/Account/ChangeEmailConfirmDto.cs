﻿namespace EzioLearning.Share.Dto.Account
{
    public class ChangeEmailConfirmDto
    {
        public string VerifyCode { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

    }
}