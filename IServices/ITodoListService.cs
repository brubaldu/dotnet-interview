using Domain;

namespace IServices;

public interface ITodoListService
{
    public Task<IEnumerable<TodoList>> GetAllAsync();
    public Task<TodoList> GetAsync(long id);
    public Task<bool> ExistsAsync(long id);
    public Task CreateAsync(TodoList todoList);
    public Task UpdateAsync(long id, TodoList todoList);
    public Task DeleteAsync(long id);
}