using System;
using System.Linq;

namespace EzioLearning.Core.Dtos.Auth
{
    public class RegisterRequestClientDto : RegisterRequestDto
    {
        public bool AllowPolicy { get; set; }

    }
}
