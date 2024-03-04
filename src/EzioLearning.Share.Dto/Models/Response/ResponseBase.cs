using System.Net;

namespace EzioLearning.Share.Models.Response;

//Merge with fluent validator
public class ResponseBase
{
    public string Type { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public HttpStatusCode Status { get; init; }
    public string? Message { get; init; }

    public bool IsSuccess
    {
        get
        {
            var statusCode = (int)Status;
            return statusCode < 400;
            ;
        }
    }

    public Dictionary<string, string[]> Errors { get; init; } = [];

    public string TraceId { get; init; } = string.Empty;
}