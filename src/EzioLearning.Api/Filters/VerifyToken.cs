﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using EzioLearning.Api.Models.Response;
using EzioLearning.Api.Services;
using EzioLearning.Api.Models.Constants;

namespace EzioLearning.Api.Filters
{
    public class VerifyTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var memoryCache = context.HttpContext.RequestServices.GetService<CacheService>();

            var sessionId = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value ?? string.Empty;

            var token = memoryCache?.Get<string>(sessionId, CacheConstant.AccessToken);

            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(token))
            {
                context.Result = new BadRequestObjectResult(new ResponseBase
                {
                    Message = "Token giả mạo!",
                    StatusCode = 400
                });
            }
            base.OnActionExecuting(context);
        }
    }
}
