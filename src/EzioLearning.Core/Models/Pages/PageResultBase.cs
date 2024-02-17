namespace EzioLearning.Core.Models.Pages
{
    public class PageResultBase
    {
        public int CurrentPage { get; set; }
        public int PageCount
        {
            get
            {
                var pageCount = RowCount * 1.0 / PageSize;
                return (int)Math.Ceiling(pageCount);
            }
            protected set => PageCount = value > 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
        }

        public int PageSize { get; set; }

        public int RowCount { get; set; }

        public bool IsFirstPage => CurrentPage is 1;
        public bool IsLastPage => CurrentPage == PageCount;

    }
}
