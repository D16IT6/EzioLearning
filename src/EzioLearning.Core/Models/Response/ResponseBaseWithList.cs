namespace EzioLearning.Core.Models.Response
{
	public class ResponseBaseWithList<TItem> : ResponseBase
	{
		public IEnumerable<TItem>? Data { get; set; }

	}
}
