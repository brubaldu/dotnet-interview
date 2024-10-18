using Domain;

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

    private async Task TodoListExists(long id)
    {
        if (await _todoContext.TodoLists.FindAsync(id) == null)
            throw new KeyNotFoundException();
    }
}