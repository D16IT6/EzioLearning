using System.Net;

namespace EzioLearning.Core.Models.Response
{
    //Merge with fluent validator
    public class ResponseBase
    {
        public string Type { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;
        public HttpStatusCode Status { get; init; }
        public string? Message { get; init; }

        public bool IsSuccess
        {
            get
            {
                int statusCode = (int)Status;
                return statusCode < 400;
                ;
            }
        }

        public Dictionary<string, string[]> Errors { get; set; } = [];

        public string TraceId { get; set; } = String.Empty;

    }
}
