using System.Collections;
using Domain;
using IRepository;
using IServices;

namespace Services;

public class TodoItemService:ITodoItemService
{
    private readonly IRepository<TodoItem> _todoItemRepository;

    public TodoItemService(IRepository<TodoItem> todoItemRepository)
    {
        _todoItemRepository = todoItemRepository;
    }
    

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        return await _todoItemRepository.GetAllAsync();
    }

    public async Task<IEnumerable<TodoItem>> GetAllByTodoListIdAsync(long todoListId)
    {
        return await _todoItemRepository.GetAllByAsync(t => t.TodoListId == todoListId);
    }

    public async Task<TodoItem> GetAsync(long todoListId, long todoItemId)
    {
        return await _todoItemRepository.FindAsync(t=> t.TodoListId == todoListId && t.Id == todoItemId);
    }

    public async Task<bool> ExistsAsync(long todoItemId)
    {
        return await _todoItemRepository.ExistsAsync(t=> t.Id == todoItemId);
    }

    public async Task<bool> ExistsAsync(long todoListId, long todoItemId)
    {
        return await _todoItemRepository.ExistsAsync(t=> t.TodoListId == todoListId && t.Id == todoItemId);
    }

    public async Task DeleteAsync(long id)
    {
        await _todoItemRepository.DeleteAsync(id);
    }

    public async Task UpdateAsync(long id, TodoItem TodoItem)
    {
        await _todoItemRepository.UpdateAsync(id, TodoItem);
    }

    public async Task CreateAsync(TodoItem TodoItem)
    {
        await _todoItemRepository.InsertAsync(TodoItem);
    }
}