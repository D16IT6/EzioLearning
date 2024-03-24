using EzioLearning.Share.Models.Pages;
using System.Linq.Expressions;

namespace EzioLearning.Core.SeedWorks;

public interface IPagedRepository<T, in TKey> : IRepository<T, TKey>
    where T : class
{
    Task<PageResult<TDto>> GetPage<TDto>(Expression<Func<T, bool>>? expression = default, int pageNumber = 1,
        int pageSize = 10) where TDto : class;

    Task<IEnumerable<TDto>> GetAllWithDto<TDto>(Expression<Func<T, bool>>? expression = default);
}