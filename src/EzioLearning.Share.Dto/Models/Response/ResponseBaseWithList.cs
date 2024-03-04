namespace EzioLearning.Share.Models.Response;

public class ResponseBaseWithList<TItem> : ResponseBase
{
    public List<TItem>? Data { get; init; } = new();
}