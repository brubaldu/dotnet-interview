using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repository;
public class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public DbSet<TodoList> TodoLists { get; set; } = default!;
    public DbSet<TodoItem> TodoItems { get; set; }
}
