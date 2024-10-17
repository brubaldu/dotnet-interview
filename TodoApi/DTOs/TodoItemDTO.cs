using TodoApi.Models;

namespace TodoApi.DTOs;

public class TodoItemDTO
{
    public long Id { get; set; }
    public string Text { get; set; }
    public long TodoListId { get; set; }

    public TodoItemDTO()
    {
    }

    public TodoItemDTO(TodoItem todoItem)
    {
        Id = todoItem.Id;
        Text = todoItem.Text;
        TodoListId = todoItem.TodoListId;
    }

    public TodoItem ToEntity()
    {
        return new TodoItem()
        {
            Id = Id,
            Text = Text,
            TodoListId = TodoListId
        };
    }
}