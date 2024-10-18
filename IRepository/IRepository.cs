using System.Linq.Expressions;

namespace IRepository;

public interface IRepository<T> where T : class
{
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<IEnumerable<T>> GetAllByAsync(Expression<Func<T, bool>> expression);
    public Task<T?> FindAsync(long id);
    public Task<T?> FindAsync(Expression<Func<T, bool>> expression);
    public Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
    public Task InsertAsync(T elem);
    public Task DeleteAsync(long id);
    public Task UpdateAsync(long id, T elem);
}