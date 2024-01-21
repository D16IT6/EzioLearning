﻿
using EzioLearning.Core.Models;
using System.Linq.Expressions;

namespace EzioLearning.Core.SeedWorks
{
    public interface IPagedRepository<T, in TKey, TDto> : IRepository<T, TKey>
        where TDto : class
        where T : class
    {
        Task<PageResult<TDto>> GetPage(Expression<Func<T, bool>>? expression, int pageNumber = 1, int pageSize = 10);
    }
}