using System.Linq.Expressions;

namespace EzioLearning.Core.SeedWorks;

public interface IRepository<T, in TKey> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(string[]? includes = null);
    Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression, string[]? includes = null);
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}