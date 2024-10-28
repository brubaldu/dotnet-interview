using Domain;
using Microsoft.EntityFrameworkCore;


namespace Repository.Tests;

public class TodoItemRepositoryTests
{
    private DbContextOptions<TodoContext> DatabaseContextOptions()
    {
        return new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private void PopulateDatabaseContext(TodoContext context)
    {
    
        context.TodoLists.Add(new TodoList { Id = 1, Name = "Task 1", Items = new List<TodoItem>()
            {
                new TodoItem(){Id = 1, Text = "Item 1", TodoListId = 1, Created = new DateTime(2024,10,01)}, 
                new TodoItem(){Id = 2, Text = "Item 2", TodoListId = 1, Created = new DateTime(2024,10,01)}
            }
        });
        context.TodoLists.Add(new TodoList { Id = 2, Name = "Task 2" });
        context.SaveChanges();
        var todoList3 = new TodoList { Id = 3, Name = "List 3" };
    
        // Adding 1998 TodoItems for testing
        for (int i = 3; i < 1000; i++)
        {
            var todoItem = new TodoItem { Id = i, Text = $"Text {i}", Created = new DateTime(2023,01,01)};
            todoList3.Items.Add(todoItem);
        }
        context.TodoLists.Add(todoList3);
    
        var todoList4 = new TodoList { Id = 4, Name = "List 4" };
        for (int i = 1; i < 1000; i++)
        {
            var id = 1000 + i;
            var todoItem = new TodoItem { Id = id, Text = $"Text {id}", Created = new DateTime(2024,01,02)};
            todoList4.Items.Add(todoItem);
        }
        context.TodoLists.Add(todoList4);
        context.SaveChanges();
    }
    
    [Fact]
    public async Task CleanOldData_WhenCalledWith10Rows_ReturnsTrueAndDeletes10()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);
            var repository = new TodoItemRepository(context);
            var dateFrom = new DateTime(2024, 01, 01);
            var result = await repository.CleanOldData(dateFrom, 10);
            
            Assert.True(result);

            var actualItems = (await repository.GetAllAsync()).Count();
            Assert.Equal(1988, actualItems);
        }
    }
    
    [Fact]
    public async Task CleanOldData_WhenCalledWith100Rows_ReturnsTrueAndDeletes100()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);
            var repository = new TodoItemRepository(context);
            var dateFrom = new DateTime(2024, 01, 01);
            var result = await repository.CleanOldData(dateFrom, 100);
            
            Assert.True(result);

            var actualItems = (await repository.GetAllAsync()).Count();
            Assert.Equal(1898, actualItems);
        }
    }
    
    [Fact]
    public async Task CleanOldData_WhenCalledWith1000Rows_ReturnsFalseAndDeletes997()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);
            var repository = new TodoItemRepository(context);
            var dateFrom = new DateTime(2024, 01, 01);
            var result = await repository.CleanOldData(dateFrom, 1000);
            
            Assert.False(result);

            var actualItems = (await repository.GetAllAsync()).Count();
            Assert.Equal(1001, actualItems);
        }
    }
    
    [Fact]
    public async Task CleanOldData_WhenCalledWithLaterDateAnd2000Rows_ReturnsFalseAndDeletesAllBut2()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);
            var repository = new TodoItemRepository(context);
            var dateFrom = new DateTime(2024, 6, 01);
            var result = await repository.CleanOldData(dateFrom, 2000);
            
            Assert.False(result);

            var actualItems = (await repository.GetAllAsync()).Count();
            Assert.Equal(2, actualItems);
        }
    }
    
    [Fact]
    public async Task CleanOldData_WhenCalledWithEvenLaterDateAnd2000Rows_ReturnsFalseAndDeletesAll()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);
            var repository = new TodoItemRepository(context);
            var dateFrom = new DateTime(2024, 10, 20);
            var result = await repository.CleanOldData(dateFrom, 2000);
            
            Assert.False(result);

            var actualItems = (await repository.GetAllAsync()).Count();
            Assert.Equal(0, actualItems);
        }
    }
    
    [Fact]
    public async Task CleanOldData_WhenCalledWithNonExistingDate_ReturnsFalseAndDeletes0()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);
            var repository = new TodoItemRepository(context);
            var dateFrom = new DateTime(1999, 01, 01);
            var result = await repository.CleanOldData(dateFrom, 100);
            
            Assert.False(result);

            var actualItems = (await repository.GetAllAsync()).Count();
            Assert.Equal(1998, actualItems);
        }
    }
}