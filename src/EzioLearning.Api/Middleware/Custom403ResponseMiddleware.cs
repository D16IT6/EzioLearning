using System.Net;
using System.Text.Json;
using EzioLearning.Share.Models.Response;

namespace EzioLearning.Api.Middleware
{
    public class Custom403ResponseMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next(context);

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";

                var message = "Bạn không có quyền truy cập chức năng này";
                var response = new ResponseBase
                {
                    Status = HttpStatusCode.Forbidden,
                    Message = message,
                    Errors = new Dictionary<string, string[]>
                    {
                        { "UnAuthorize", [message] }
                    }
                };
                var jsonResponse = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }


        }
    }
}
