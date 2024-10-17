using TodoApi.Controllers;
using TodoApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.DTOs;

namespace TodoApi.Tests.Controllers;

#nullable disable
public class TodoItemControllerTests
{
  private DbContextOptions<TodoContext> DatabaseContextOptions()
  {
    return new DbContextOptionsBuilder<TodoContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;
  }

  private void PopulateDatabaseContext(TodoContext context)
  {
    
    context.TodoLists.Add(new Models.TodoList { Id = 1, Name = "Task 1", Items = new List<TodoItem>()
      {
        new TodoItem(){Id = 1, Text = "Item 1", TodoListId = 1}, 
        new TodoItem(){Id = 2, Text = "Item 2", TodoListId = 1}
      }
    });
    context.TodoLists.Add(new Models.TodoList { Id = 2, Name = "Task 2" });
    context.SaveChanges();
  }

  [Fact]
  public async Task GetTodoItem_WhenCalled_ReturnsTodoItemList()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = new TodoItemsController(context);

      var result = await controller.GetTodoItems(1);
      
      Assert.IsType<OkObjectResult>(result.Result);
      Assert.Equal(
        2,
        ((result.Result as OkObjectResult).Value as IList<TodoItemDTO>).Count
      );
    }
  }

  [Fact]
  public async Task GetTodoItem_WhenCalled_ReturnsTodoItemById()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = new TodoItemsController(context);

      var result = await controller.GetTodoItem(1,1);

      Assert.IsType<OkObjectResult>(result.Result);
      Assert.Equal(
        1,
        ((result.Result as OkObjectResult).Value as TodoItemDTO).Id
      );
    }
  }

  [Fact]
  public async Task PutTodoItem_WhenTodoItemIdDoesntMatch_ReturnsBadRequest()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = new TodoItemsController(context);

      var todoItem = await context.TodoItems.Where(x => x.Id == 2).FirstAsync();
      var result = await controller.PutTodoItem(1,1, new TodoItemDTO(todoItem));

      Assert.IsType<BadRequestResult>(result);
    }
  }

  [Fact]
  public async Task PutTodoItem_WhenTodoItemDoesntExist_ReturnsBadRequest()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = new TodoItemsController(context);

      var result = await controller.PutTodoItem(1,3, new TodoItemDTO() { Id = 3 });

      Assert.IsType<NotFoundResult>(result);
    }
  }

  [Fact]
  public async Task PutTodoItem_WhenCalled_UpdatesTheTodoItem()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = new TodoItemsController(context);
      
      var todoItemDto = new TodoItemDTO() { Id = 1, Text = "New Text", TodoListId = 1 };
      
      var result = await controller.PutTodoItem(1, todoItemDto.Id, todoItemDto);

      Assert.IsType<NoContentResult>(result);
    }
  }

  [Fact]
  public async Task PostTodoItem_WhenCalled_CreatesTodoItem()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = new TodoListsController(context);

      var todoList = new CreateTodoListDTO() { Name = "Task 3" };
      var result = await controller.PostTodoList(todoList);

      Assert.IsType<CreatedAtActionResult>(result.Result);
      Assert.Equal(
        3,
        context.TodoLists.Count()
      );
    }
  }

  [Fact]
  public async Task DeleteTodoList_WhenCalled_RemovesTodoList()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = new TodoListsController(context);

      var result = await controller.DeleteTodoList(2);

      Assert.IsType<NoContentResult>(result);
      Assert.Equal(
        1,
        context.TodoLists.Count()
      );
    }
  }
}