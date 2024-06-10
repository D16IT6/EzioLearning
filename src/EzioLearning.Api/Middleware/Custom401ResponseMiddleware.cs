using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using EzioLearning.Share.Models.Response;
using Serilog;

namespace EzioLearning.Api.Middleware;

public class Custom401ResponseMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        await next(context);

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            var response = new ResponseBase
            {
                Message = "Bạn chưa xác thực",
                Errors =
                {
                    { "UnAuthorized", ["Bạn chưa xác thực"] }
                },
                Status = HttpStatusCode.Unauthorized,
                Type = HttpResponseType.Unauthorized
            };

            context.Response.ContentType = "application/json";


            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
            Log.Error("Lỗi từ custom 401 middleware");

        }
    }
}