using System.Collections.Specialized;
using System.Reflection;
using System.Web;

namespace EzioLearning.Share.Models.Request
{
    public class CourseListOptions
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 1;
        public string? SearchText { get; set; } = string.Empty;
        public List<Guid> CourseCategoryIds { get; set; } = [];
        public PriceType PriceType { get; set; } = PriceType.All;

        public NameValueCollection CreateQueryString()
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["PageNumber"] = PageNumber.ToString();
            queryString["PageSize"] = PageSize.ToString();
            queryString["SearchText"] = SearchText ?? string.Empty;

            foreach (var categoryId in CourseCategoryIds)
            {
                queryString.Add("CourseCategoryIds", categoryId.ToString());
            }

            queryString["PriceType"] = ((int)PriceType).ToString();

            return queryString;
        }
    }

    public enum PriceType
    {
        Free,
        Paid,
        All
    }
}
