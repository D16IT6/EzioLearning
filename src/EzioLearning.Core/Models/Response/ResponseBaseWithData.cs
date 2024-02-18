namespace EzioLearning.Core.Models.Response
{
    public class ResponseBaseWithData<TItem> : ResponseBase where TItem : class, new() 
    {
        public TItem Data { get; set; } = new();
    }
}
