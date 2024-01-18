using System.Linq.Expressions;
using EzioLearning.Core.Models;
using EzioLearning.Core.SeedWorks;
using EzioLearning.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace EzioLearning.Infrastructure.SeedWorks
{
    public class RepositoryBase<T, TKey>(EzioLearningDbContext context) : IRepository<T, TKey>
        where T : class
    {
        protected readonly DbSet<T> DbSet = context.Set<T>();

        public async Task<T?> GetByIdAsync(TKey key)
        {
            return await DbSet.FindAsync(key);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return DbSet.Where(expression);
        }

        public void Add(T entity)
        {
            DbSet.AddAsync(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            DbSet.AddRangeAsync(entities);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}
