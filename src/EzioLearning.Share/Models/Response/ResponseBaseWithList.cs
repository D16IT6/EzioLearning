namespace EzioLearning.Share.Models.Response;

public class ResponseBaseWithList<TItem> : ResponseBase
{
    public IEnumerable<TItem> Data { get; init; } = [];
}