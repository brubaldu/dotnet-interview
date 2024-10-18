using System.Collections;
using Domain;
using IRepository;
using IServices;

namespace Services;

public class TodoListService:ITodoListService
{
    private readonly IRepository<TodoList> _todoListRepository;

    public TodoListService(IRepository<TodoList> todoListRepository)
    {
        _todoListRepository = todoListRepository;
    }
    

    public async Task<IEnumerable<TodoList>> GetAllAsync()
    {
        return await _todoListRepository.GetAllAsync();
    }

    public async Task<TodoList> GetAsync(long id)
    {
        return await _todoListRepository.FindAsync(id);
    }

    public async Task<bool> ExistsAsync(long id)
    {
        return await _todoListRepository.ExistsAsync(t=>t.Id == id);
    }

    public async Task DeleteAsync(long id)
    {
        await _todoListRepository.DeleteAsync(id);
    }

    public async Task UpdateAsync(long id, TodoList todoList)
    {
        await _todoListRepository.UpdateAsync(id, todoList);
    }

    public async Task CreateAsync(TodoList todoList)
    {
        await _todoListRepository.InsertAsync(todoList);
    }
}