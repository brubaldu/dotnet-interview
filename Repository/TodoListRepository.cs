using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class TodoListRepository : BaseRepository<TodoList>
{
    public TodoListRepository(TodoContext todoContext) : base(todoContext)
    {
    }

    public override async Task<TodoList?> FindAsync(long id)
    {
        var todoList = await _todoContext.Set<TodoList>().Include(t=>t.Items)
            .FirstOrDefaultAsync(t=>t.Id==id);
        if (todoList == null)
            throw new KeyNotFoundException();
        return todoList;
    }
}