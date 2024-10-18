using Domain;
using TodoApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using TodoApi.DTOs;

namespace TodoApi.Tests.Controllers;

#nullable disable
public class TodoListsControllerTests
{
  private DbContextOptions<TodoContext> DatabaseContextOptions()
  {
    return new DbContextOptionsBuilder<TodoContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;
  }

  private void PopulateDatabaseContext(TodoContext context)
  {
    context.TodoLists.Add(new TodoList { Id = 1, Name = "Task 1" });
    context.TodoLists.Add(new TodoList { Id = 2, Name = "Task 2" });
    context.SaveChanges();
  }

  private TodoListsController CreateController(TodoContext context)
  {
    var repository = new TodoListRepository(context);
    var todoListService = new TodoListService(repository);
    return new TodoListsController(todoListService);
  }

  [Fact]
  public async Task GetTodoList_WhenCalled_ReturnsTodoListList()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var result = await controller.GetTodoLists();

      Assert.IsType<OkObjectResult>(result.Result);
      Assert.Equal(
        2,
        ((result.Result as OkObjectResult).Value as IList<TodoListListDTO>).Count
      );
    }
  }

  [Fact]
  public async Task GetTodoList_WhenCalled_ReturnsTodoListById()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var result = await controller.GetTodoList(1);

      Assert.IsType<OkObjectResult>(result.Result);
      Assert.Equal(
        1,
        ((result.Result as OkObjectResult).Value as TodoListDTO).Id
      );
    }
  }
  
  [Fact]
  public async Task GetTodoList_WhenCalledWithWrongId_ReturnsNotFound()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var result = await controller.GetTodoList(20);

      Assert.IsType<NotFoundResult>(result.Result);
    }
  }

  [Fact]
  public async Task PutTodoList_WhenTodoListIdDoesntMatch_ReturnsBadRequest()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var todoList = await context.TodoLists.Where(x => x.Id == 2).FirstAsync();
      var result = await controller.PutTodoList(1, todoList);

      Assert.IsType<BadRequestResult>(result);
    }
  }

  [Fact]
  public async Task PutTodoList_WhenTodoListDoesntExist_ReturnsBadRequest()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var result = await controller.PutTodoList(3, new TodoList { Id = 3});

      Assert.IsType<NotFoundResult>(result);
    }
  }

  [Fact]
  public async Task PutTodoList_WhenCalled_UpdatesTheTodoList()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var todoList = await context.TodoLists.Where(x => x.Id == 2).FirstAsync();
      var result = await controller.PutTodoList(todoList.Id, todoList);

      Assert.IsType<NoContentResult>(result);
    }
  }

  [Fact]
  public async Task PostTodoList_WhenCalled_CreatesTodoList()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

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
  public async Task PostTodoItem_WithExistingId_ReturnsProblem()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);
  
      var controller = CreateController(context);
  
      var todoListDto = new CreateTodoListDTO() {Id = 1, Name = "A Name" };

      var result = await controller.PostTodoList(todoListDto);
      
      Assert.IsType<ObjectResult>(result.Result);
      Assert.Equal(
        409,
        (result.Result as ObjectResult).StatusCode
      );
      
    }
  }

  [Fact]
  public async Task DeleteTodoList_WhenCalled_RemovesTodoList()
  {
    using (var context = new TodoContext(DatabaseContextOptions()))
    {
      PopulateDatabaseContext(context);

      var controller = CreateController(context);

      var result = await controller.DeleteTodoList(2);

      Assert.IsType<NoContentResult>(result);
      Assert.Equal(
        1,
        context.TodoLists.Count()
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
  
      var result = await controller.DeleteTodoList(20);
  
      Assert.IsType<NotFoundResult>(result);
    }
  }
}