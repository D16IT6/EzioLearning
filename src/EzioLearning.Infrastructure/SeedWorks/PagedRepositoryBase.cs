using System.Linq.Expressions;
using AutoMapper;
using EzioLearning.Core.Models.Pages;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.SeedWorks
{
    public class PagedRepositoryBase<T, TKey, TDto>(EzioLearningDbContext context, IMapper mapper)
        : RepositoryBase<T, TKey>(context), IPagedRepository<T, TKey, TDto>
        where TDto : class
        where T : class
    {
        public async Task<PageResult<TDto>> GetPage(Expression<Func<T, bool>>? expression, int pageNumber = 1, int pageSize = 10)
        {

            var query = DbSet.AsQueryable();
            if (expression != null)
            {
                query = query.Where(expression);
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return new PageResult<TDto>()
            {
                Data = await mapper.ProjectTo<TDto>(query).ToListAsync(),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                RowCount = await query.CountAsync(),
            };

        }
    }
}
