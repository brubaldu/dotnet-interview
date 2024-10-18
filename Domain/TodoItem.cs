namespace Domain;

public class TodoItem
{
    public long Id { get; set; }
    public string Text { get; set; }
    public long TodoListId { get; set; }
    public TodoList TodoList { get; set; }
}