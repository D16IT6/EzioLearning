using EzioLearning.Core.Models.Pages;
using System.Linq.Expressions;

namespace EzioLearning.Core.SeedWorks
{
    public interface IPagedRepository<T, in TKey, TDto>
        where TDto : class
        where T : class
    {
        Task<PageResult<TDto>> GetPage(Expression<Func<T, bool>>? expression = default, int pageNumber = 1, int pageSize = 10);
    }
}
