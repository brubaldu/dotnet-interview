using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class TodoItemRepository : BaseRepository<TodoItem>
{
    public TodoItemRepository(TodoContext todoContext) : base(todoContext)
    {
    }

    public override async Task InsertAsync(TodoItem elem)
    {
        await TodoListExists(elem.TodoListId);
        await base.InsertAsync(elem);
    }
    
    public override async Task UpdateAsync(long id, TodoItem elem)
    {
        await TodoListExists(elem.TodoListId);
        await base.UpdateAsync(id, elem);
    }

    public override async Task<bool> CleanOldData(DateTime dateFrom, int rowsToDelete)
    {
        var itemsToRemove = await _todoContext.TodoItems.Where(t => t.Created <= dateFrom)
            .Take(rowsToDelete).ToListAsync();
        _todoContext.TodoItems.RemoveRange(itemsToRemove);
        await _todoContext.SaveChangesAsync();
        return await _todoContext.TodoItems.AnyAsync(t => t.Created <= dateFrom);
    }

    private async Task TodoListExists(long id)
    {
        if (await _todoContext.TodoLists.FindAsync(id) == null)
            throw new KeyNotFoundException();
    }
}