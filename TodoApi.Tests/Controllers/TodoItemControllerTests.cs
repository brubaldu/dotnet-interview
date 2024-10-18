using Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using TodoApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
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
    
    context.TodoLists.Add(new TodoList { Id = 1, Name = "Task 1", Items = new List<TodoItem>()
      {
        new TodoItem(){Id = 1, Text = "Item 1", TodoListId = 1}, 
        new TodoItem(){Id = 2, Text = "Item 2", TodoListId = 1}
      }
    });
    context.TodoLists.Add(new TodoList { Id = 2, Name = "Task 2" });
    context.SaveChanges();
  }
  
  private TodoItemsController CreateController(TodoContext context)
  {
    var repository = new TodoItemRepository(context);
    var todoItemRepository = new TodoItemService(repository);
    return new TodoItemsController(todoItemRepository);
  }

  [Fact]
  public async Task GetTodoItem_WhenCalled_ReturnsTodoItemList()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

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

      var controller = CreateController(context);

      var result = await controller.GetTodoItem(1,1);

      Assert.IsType<OkObjectResult>(result.Result);
      Assert.Equal(
        1,
        ((result.Result as OkObjectResult).Value as TodoItemDTO).Id
      );
    }
  }
  
  [Fact]
  public async Task GetTodoItem_WhenCalledWithWrongItemId_ReturnsNotFound()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var result = await controller.GetTodoItem(1,20);

      Assert.IsType<NotFoundResult>(result.Result);
    }
  }
  
  [Fact]
  public async Task GetTodoItem_WhenCalledWithWrongListId_ReturnsNotFound()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var result = await controller.GetTodoItem(20,1);

      Assert.IsType<NotFoundResult>(result.Result);
    }
  }

  [Fact]
  public async Task PutTodoItem_WhenTodoItemIdDoesntMatch_ReturnsBadRequest()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

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

      var controller = CreateController(context);

      var result = await controller.PutTodoItem(1,3, new TodoItemDTO() { Id = 3 });

      Assert.IsType<NotFoundResult>(result);
    }
  }
  
  [Fact]
  public async Task PutTodoItem_WhenCalledWithNonExistingTodoListId_ThrowsKeyNotFoundException()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);
  
      var controller = CreateController(context);
  
      var todoItem = new TodoItemDTO() { Id = 1, Text = "Text 3", TodoListId = 20};

      await Assert.ThrowsAsync<KeyNotFoundException>(() => controller.PutTodoItem(1,1, todoItem));
    }
  }

  [Fact]
  public async Task PutTodoItem_WhenCalled_UpdatesTheTodoItemText()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var newText = "New Text";
      var todoItemDto = new TodoItemDTO() { Id = 1, Text = newText, TodoListId = 1 };
      
      var result = await controller.PutTodoItem(1, todoItemDto.Id, todoItemDto);

      Assert.IsType<NoContentResult>(result);
      Assert.Equal(newText,context.TodoItems.Find((long)1).Text);
    }
  }
  
  [Fact]
  public async Task PutTodoItem_WhenCalled_UpdatesTheTodoItemTodoListId()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      long newTodoListId = 2;
      var todoItemDto = new TodoItemDTO() { Id = 1, Text = "Item 1", TodoListId = newTodoListId };
      
      var result = await controller.PutTodoItem(1, todoItemDto.Id, todoItemDto);

      Assert.IsType<NoContentResult>(result);
      Assert.Equal(newTodoListId, context.TodoItems.Find((long)1).TodoListId);
    }
  }

  [Fact]
  public async Task PostTodoItem_WhenCalled_CreatesTodoItem()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);
  
      var controller = CreateController(context);
  
      var todoItem = new CreateTodoItemDTO() { Text = "Text 3" };
      var result = await controller.PostTodoItem(1, todoItem);
  
      Assert.IsType<CreatedAtActionResult>(result.Result);
      Assert.Equal(
        3,
        context.TodoItems.Count()
      );
    }
  }
  
  [Fact]
  public async Task PostTodoItem_WhenCalledWithNonExistingTodoListId_ThrowsKeyNotFoundException()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);
  
      var controller = CreateController(context);
  
      var todoItem = new CreateTodoItemDTO() { Text = "Text 3" };

      await Assert.ThrowsAsync<KeyNotFoundException>(() => controller.PostTodoItem(20, todoItem));
    }
  }
  
  [Fact]
  public async Task PostTodoItem_WithExistingId_ReturnsProblem()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);
  
      var controller = CreateController(context);
  
      var todoitemDto = new CreateTodoItemDTO() {Id = 1, Text = "Text 3" };

      var result = await controller.PostTodoItem(1, todoitemDto);
      
      Assert.IsType<ObjectResult>(result.Result);
      Assert.Equal(
        409,
        (result.Result as ObjectResult).StatusCode
      );
      
    }
  }
  
  [Fact]
  public async Task DeleteTodoItem_WhenCalled_RemovesTodoItem()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);
  
      var controller = CreateController(context);
  
      var result = await controller.DeleteTodoItem(1,2);
  
      Assert.IsType<NoContentResult>(result);
      Assert.Equal(
        1,
        context.TodoItems.Count()
      );
    }
  }
  
  [Fact]
  public async Task DeleteTodoItem_WhenCalledWithNonExistingId_ReturnsNotFound()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);
  
      var controller = CreateController(context);
  
      var result = await controller.DeleteTodoItem(1,20);
  
      Assert.IsType<NotFoundResult>(result);
    }
  }
  
  [Fact]
  public async Task DeleteTodoItem_WhenCalledWithNonExistingTodoListId_ReturnsNotFound()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);
  
      var controller = CreateController(context);
  
      var result = await controller.DeleteTodoItem(20,1);
  
      Assert.IsType<NotFoundResult>(result);
    }
  }
}