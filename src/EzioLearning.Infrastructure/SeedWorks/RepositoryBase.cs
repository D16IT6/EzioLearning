using System.Linq.Expressions;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.SeedWorks;

public class RepositoryBase<T, TKey>(EzioLearningDbContext context) : IRepository<T, TKey>
    where T : class
{

    protected readonly DbSet<T> DbSet = context.Set<T>();

    public virtual Task<IEnumerable<T>> GetAllAsync(string[]? includes = null)
    {
        var query = DbSet.AsQueryable();

        if (includes != null && includes.Any())
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        return Task.FromResult(query.AsEnumerable());
    }

    public virtual Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression, string[]? includes = null)
    {
        var query = DbSet.AsQueryable();

        if (includes != null && includes.Any())
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        query = query.Where(expression);
        return Task.FromResult(query.AsEnumerable());
    }

    public virtual void Add(T entity)
    {
        DbSet.AddAsync(entity);
    }

    public virtual void AddRange(IEnumerable<T> entities)
    {
        DbSet.AddRangeAsync(entities);
    }



    public virtual void Remove(T entity)
    {
        DbSet.Remove(entity);
    }

    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        DbSet.RemoveRange(entities);
    }
}