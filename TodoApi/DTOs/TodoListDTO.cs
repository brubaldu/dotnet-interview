using Domain;

namespace TodoApi.DTOs;

public class TodoListDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public List<TodoItemListDTO> TodoListItem { get; set; }

    public TodoListDTO(TodoList todoList)
    {
        Id = todoList.Id;
        Name = todoList.Name;
        TodoListItem = todoList.Items.Select(i => new TodoItemListDTO(i)).ToList();
    }
}