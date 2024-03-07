using System.Linq.Expressions;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.SeedWorks;

public class RepositoryBase<T, TKey>(EzioLearningDbContext context) : IRepository<T, TKey>
    where T : class
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(TKey key)
    {
        return await DbSet.FindAsync(key);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.AsNoTracking().ToListAsync();
    }

    public virtual IEnumerable<T> Find(Expression<Func<T, bool>> expression)
    {
        return DbSet.Where(expression);
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