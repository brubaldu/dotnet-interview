using System.Linq.Expressions;
using IRepository;
using Microsoft.EntityFrameworkCore;

namespace Repository;


public class BaseRepository<T>: IRepository<T> where T :class
{
    protected readonly TodoContext _todoContext;

    public BaseRepository(TodoContext todoContext)
    {
        _todoContext = todoContext;
    }


    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _todoContext.Set<T>().ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllByAsync(Expression<Func<T, bool>> expression)
    {
        return await _todoContext.Set<T>().Where(expression).ToListAsync();
    }

    public virtual async Task<T?> FindAsync(long id)
    {
        return await _todoContext.Set<T>().FindAsync(id);
    }
    
    public async Task<T?> FindAsync(Expression<Func<T, bool>> expression)
    {
        return await _todoContext.Set<T>().Where(expression).FirstOrDefaultAsync();
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
    {
        return await _todoContext.Set<T>().Where(expression).FirstOrDefaultAsync() != null;
    }

    public virtual async Task InsertAsync(T elem)
    {
        await _todoContext.Set<T>().AddAsync(elem);
        await _todoContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var elem = await _todoContext.Set<T>().FindAsync(id);
        if (elem == null)
            throw new KeyNotFoundException();
        _todoContext.Set<T>().Remove(elem);
        await _todoContext.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(long id, T elem)
    {
        var elemToUpdate = await _todoContext.Set<T>().FindAsync(id);
        if (elemToUpdate == null)
            throw new KeyNotFoundException();
        _todoContext.Entry(elemToUpdate).CurrentValues.SetValues(elem);
        await _todoContext.SaveChangesAsync();
    }

    public virtual async Task<bool> CleanOldData(DateTime dateFrom, int rowsToDelete)
    {
        throw new NotImplementedException();
    }
}