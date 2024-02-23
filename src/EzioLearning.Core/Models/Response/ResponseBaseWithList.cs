namespace EzioLearning.Core.Models.Response
{
	public class ResponseBaseWithList<TItem> : ResponseBase 
    {
        public List<TItem>? Data { get; set; } = new();

    }
}
