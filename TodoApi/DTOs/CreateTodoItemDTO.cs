using Domain;

namespace TodoApi.DTOs;

public class CreateTodoItemDTO
{
    public long Id { get; set; }
    public string Text { get; set; }

    public TodoItem ToEntity()
    {
        return new TodoItem()
        {
            Id = Id,
            Text = Text
        };
    }
}