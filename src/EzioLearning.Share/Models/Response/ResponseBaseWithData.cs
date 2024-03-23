namespace EzioLearning.Share.Models.Response;

public class ResponseBaseWithData<TItem> : ResponseBase
{
    public TItem? Data { get; init; }
}