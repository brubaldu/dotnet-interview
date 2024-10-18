using Domain;

namespace TodoApi.DTOs;

public class CreateTodoListDTO
{
    public long Id { get; set; }
    public string Name { get; set; }

    public TodoList ToEntity()
    {
        return new TodoList()
        {
            Id = Id,
            Name = Name,
            Items = new List<TodoItem>()
        };
    }
}