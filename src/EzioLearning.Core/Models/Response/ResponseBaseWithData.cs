namespace EzioLearning.Core.Models.Response
{
    public class ResponseBaseWithData<TItem> : ResponseBase
    {
        public TItem? Data { get; set; } 
    }
}
