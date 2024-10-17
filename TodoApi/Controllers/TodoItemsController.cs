using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Controllers
{
  [Route("api/todolists/{todoListId}/todoitems")]
  [ApiController]
  public class TodoItemsController : ControllerBase
  {
    private readonly TodoContext _context;

    public TodoItemsController(TodoContext context)
    {
      _context = context;
    }

    // GET: api/todolists/5/todoitems
    [HttpGet]
    public async Task<ActionResult<IList<TodoItemDTO>>> GetTodoItems(long todoListId)
    {
      if (_context.TodoItems == null)
      {
        return NotFound();
      }

      var todoItems = await _context.TodoItems.Where(i => i.TodoListId == todoListId).ToListAsync();

      return Ok(todoItems.Select(t=>new TodoItemDTO(t)).ToList());
    }

    // GET: api/todolists/5/todoitems/1
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long todoListId, int id)
    {
      if (_context.TodoItems == null)
      {
        return NotFound();
      }

      var todoItem = await _context.TodoItems.Where(i=> i.TodoListId==todoListId && i.Id==id).FirstOrDefaultAsync();

      if (todoItem == null)
      {
        return NotFound();
      }

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
      
      var existingTodoItem = await _context.TodoItems
        .Where(i=> i.TodoListId==todoListId && i.Id==id).FirstOrDefaultAsync();
      
      if (existingTodoItem == null)
      {
        return NotFound();
      }

      existingTodoItem.Text = todoItemDto.Text;
      existingTodoItem.TodoListId = todoItemDto.TodoListId;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!TodoItemExists(id))
        {
          return NotFound();
        }
        else
        {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/todolists/5/todoitems
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(long todoListId, CreateTodoItemDTO todoItemDto)
    {
      if (_context.TodoItems == null)
      {
        return Problem("Entity set 'TodoContext.TodoItem'  is null.");
      }

      TodoItem todoItem = todoItemDto.ToEntity();
      todoItem.TodoListId = todoListId;
      _context.TodoItems.Add(todoItem);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetTodoItem), new {todoListId = todoListId, id = todoItemDto.Id },
        new TodoItemDTO(todoItem));
    }

    // DELETE: api/todolists/5/todoitems/1
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoItem(long todoListId, long id)
    {
      if (_context.TodoItems == null)
      {
        return NotFound();
      }
      var todoList = await _context.TodoItems.Where(i=> i.TodoListId==todoListId && i.Id==id).FirstOrDefaultAsync();
      if (todoList == null)
      {
        return NotFound();
      }

      _context.TodoItems.Remove(todoList);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool TodoItemExists(long id)
    {
      return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
    }
  }
}
