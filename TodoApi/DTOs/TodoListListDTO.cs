using TodoApi.Models;

namespace TodoApi.DTOs;

public class TodoListListDTO
{
    public long Id { get; set; }
    public string Name { get; set; }

    public TodoListListDTO(TodoList todoList)
    {
        Id = todoList.Id;
        Name = todoList.Name;
    }
}