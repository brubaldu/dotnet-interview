using System.Net;
using Domain;
using IServices;
using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;
using TodoApi.Middleware;

namespace TodoApi.Controllers
{
  [Route("api/todolists/{todoListId}/todoitems")]
  [ApiController]
  [ServiceFilter(typeof(ExceptionFilter))]
  public class TodoItemsController : ControllerBase
  {
    private readonly ITodoItemService _todoItemService;

    public TodoItemsController(ITodoItemService todoItemService)
    {
      _todoItemService = todoItemService;
    }

    // GET: api/todolists/5/todoitems
    [HttpGet]
    public async Task<ActionResult<IList<TodoItemDTO>>> GetTodoItems(long todoListId)
    {
      var todoItems = await _todoItemService.GetAllByTodoListIdAsync(todoListId);

      return Ok(todoItems.Select(t=>new TodoItemDTO(t)).ToList());
    }

    // GET: api/todolists/5/todoitems/1
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long todoListId, int id)
    {
      if (!await _todoItemService.ExistsAsync(todoListId, id))
      {
        return NotFound();
      }
      
      var todoItem = await _todoItemService.GetAsync(todoListId, id);

      return Ok(new TodoItemDTO(todoItem));
    }

    // PUT: api/todolists/5/todoitems/1
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult> PutTodoItem(long todoListId, long id, TodoItemDTO todoItemDto)
    {
      if (id != todoItemDto.Id)
      {
        return BadRequest();
      }
      
      if (!await _todoItemService.ExistsAsync(todoListId,id))
      {
        return NotFound();
      }
      
      var todoItem = todoItemDto.ToEntity();

      await _todoItemService.UpdateAsync(id, todoItem);
      
      return NoContent();
    }

    // POST: api/todolists/5/todoitems
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(long todoListId, CreateTodoItemDTO todoItemDto)
    {
      if (await _todoItemService.ExistsAsync(todoItemDto.Id))
      {
        return Problem($"TodoItem with Id {todoItemDto.Id} already exist", statusCode:(int)HttpStatusCode.Conflict);
      }

      TodoItem todoItem = todoItemDto.ToEntity();
      todoItem.TodoListId = todoListId;

      await _todoItemService.CreateAsync(todoItem);

      return CreatedAtAction(nameof(GetTodoItem), new {todoListId = todoListId, id = todoItem.Id },
        new TodoItemDTO(todoItem));
    }

    // DELETE: api/todolists/5/todoitems/1
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoItem(long todoListId, long id)
    {
      if (!await _todoItemService.ExistsAsync(todoListId,id))
      {
        return NotFound();
      }

      await _todoItemService.DeleteAsync(id);
      
      return NoContent();
    }
  }
}
