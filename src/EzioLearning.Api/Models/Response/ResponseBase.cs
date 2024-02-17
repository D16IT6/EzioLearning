namespace EzioLearning.Api.Models.Response
{
    public class ResponseBase
    {
        public object? Data { get; set; }

        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; } = true;
    }
}
