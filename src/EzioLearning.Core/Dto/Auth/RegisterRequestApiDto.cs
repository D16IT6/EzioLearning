﻿using EzioLearning.Share.Dto.Auth;
using Microsoft.AspNetCore.Http;

namespace EzioLearning.Core.Dto.Auth;

public class RegisterRequestApiDto  :RegisterRequestClientDto
{
    public new IFormFile? Avatar { get; init; }

}