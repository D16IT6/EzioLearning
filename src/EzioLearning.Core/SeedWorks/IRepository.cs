using System.Linq.Expressions;

namespace EzioLearning.Core.SeedWorks
{
    public interface IRepository<T, in TKey> where T : class

    {
        Task<T?> GetByIdAsync(TKey key);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

    }
}
