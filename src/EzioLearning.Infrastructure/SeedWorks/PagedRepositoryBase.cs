using System.Linq.Expressions;
using AutoMapper;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Infrastructure.DbContext;
using EzioLearning.Share.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.SeedWorks;

public class PagedRepositoryBase<T, TKey>(EzioLearningDbContext context, IMapper mapper)
    : RepositoryBase<T, TKey>(context), IPagedRepository<T, TKey>
    where T : class
{
    public async Task<PageResult<TDto>> GetPage<TDto>(Expression<Func<T, bool>>? expression, int pageNumber = 1,
        int pageSize = 10) where TDto : class
    {
        var query = DbSet.AsQueryable();
        if (expression != null) query = query.Where(expression);

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        return new PageResult<TDto>
        {
            Data = await mapper.ProjectTo<TDto>(query).ToListAsync(),
            CurrentPage = pageNumber,
            PageSize = pageSize,
            RowCount = await query.CountAsync()
        };
    }

    public async Task<IEnumerable<TDto>> GetAllWithDto<TDto>(Expression<Func<T, bool>>? expression = default)
    {
        var query = DbSet.AsQueryable();
        if (expression != null) query = query.Where(expression);

        return await mapper.ProjectTo<TDto>(query).ToListAsync();
    }
}