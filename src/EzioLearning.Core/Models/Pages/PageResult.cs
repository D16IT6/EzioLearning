namespace EzioLearning.Core.Models.Pages
{
    public class PageResult<T> : PageResultBase where T : class
    {
        public List<T> Data { get; set; } = [];
    }
}
