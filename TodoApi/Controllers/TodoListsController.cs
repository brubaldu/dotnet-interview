using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.DTOs;
using TodoApi.Models;

namespace TodoApi.Controllers
{
  [Route("api/todolists")]
  [ApiController]
  public class TodoListsController : ControllerBase
  {
    private readonly TodoContext _context;

    public TodoListsController(TodoContext context)
    {
      _context = context;
    }

    // GET: api/todolists
    [HttpGet]
    public async Task<ActionResult<IList<TodoListListDTO>>> GetTodoLists()
    {
      if (_context.TodoLists == null)
      {
        return NotFound();
      }

      var todoLists = await _context.TodoLists.ToListAsync();

      return Ok(todoLists.Select(t=> new TodoListListDTO(t)).ToList());
    }

    // GET: api/todolists/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDTO>> GetTodoList(long id)
    {
      if (_context.TodoLists == null)
      {
        return NotFound();
      }

      var todoList = _context.TodoLists.Include(t=> t.Items).Where(t=>t.Id==id).FirstOrDefault();

      if (todoList == null)
      {
        return NotFound();
      }

      return Ok(new TodoListDTO(todoList));
    }

    // PUT: api/todolists/5
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<ActionResult> PutTodoList(long id, TodoList todoList)
    {
      if (id != todoList.Id)
      {
        return BadRequest();
      }

      _context.Entry(todoList).State = EntityState.Modified;

      try
      {
        await _context.SaveChangesAsync();
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!TodoListExists(id))
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

    // POST: api/todolists
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoList>> PostTodoList(CreateTodoListDTO todoListDTO)
    {
      if (_context.TodoLists == null)
      {
        return Problem("Entity set 'TodoContext.TodoList'  is null.");
      }

      TodoList todoList = todoListDTO.ToEntity();
      _context.TodoLists.Add(todoList);
      await _context.SaveChangesAsync();

      return CreatedAtAction("GetTodoList", new { id = todoListDTO.Id }, todoListDTO);
    }

    // DELETE: api/todolists/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoList(long id)
    {
      if (_context.TodoLists == null)
      {
        return NotFound();
      }
      var todoList = await _context.TodoLists.FindAsync(id);
      if (todoList == null)
      {
        return NotFound();
      }

      _context.TodoLists.Remove(todoList);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool TodoListExists(long id)
    {
      return (_context.TodoLists?.Any(e => e.Id == id)).GetValueOrDefault();
    }
  }
}
