using System.Linq.Expressions;
using Domain;

namespace IServices;

public interface ITodoItemService
{
    public Task<IEnumerable<TodoItem>> GetAllAsync();
    public Task<IEnumerable<TodoItem>> GetAllByTodoListIdAsync(long todoListId);
    public Task<TodoItem> GetAsync(long todoListId, long todoItemId);
    public Task<bool> ExistsAsync(long todoItemId);
    public Task<bool> ExistsAsync(long todoListId, long todoItemId);
    public Task CreateAsync(TodoItem todoList);
    public Task UpdateAsync(long id, TodoItem todoList);
    public Task DeleteAsync(long id);
}