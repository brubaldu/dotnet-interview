using Domain;

namespace TodoApi.DTOs;

public class TodoItemListDTO
{
    public long Id { get; set; }
    public string Text { get; set; }

    public TodoItemListDTO(TodoItem todoItem)
    {
        Id = todoItem.Id;
        Text = todoItem.Text;
    }
}