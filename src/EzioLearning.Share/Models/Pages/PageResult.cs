namespace EzioLearning.Share.Models.Pages;

public class PageResult<T> : PageResultBase where T : class
{
    public List<T> Data { get; set; } = [];
}