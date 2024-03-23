using System.Net;
using System.Text.Json;
using EzioLearning.Share.Models.Response;
using Serilog;

namespace EzioLearning.Api.Middleware;

public class HandleExceptionMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        context.Response.ContentType = "application/json";

        var message = exception.Message;
        var response = new ResponseBase
        {
            Status = HttpStatusCode.InternalServerError,
            Message = message,
            Errors = new Dictionary<string, string[]>
            {
                { "ServerError", ["Máy chủ lỗi, vui lòng thử lại sau", message] }
            },
            Type = HttpResponseType.InternalServerError
        };
        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
        Log.Error("Lỗi từ custom 500 middleware");

    }
}