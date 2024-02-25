using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EzioLearning.Core.Dtos.Auth
{
    public class ConfirmPasswordDto
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public string? VerifyCode { get; set; }
    }
}
