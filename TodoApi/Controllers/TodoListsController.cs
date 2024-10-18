using System.Net;
using Domain;
using IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.DTOs;
using TodoApi.Middleware;

namespace TodoApi.Controllers
{
  [Route("api/todolists")]
  [ApiController]
  [ServiceFilter(typeof(ExceptionFilter))]
  public class TodoListsController : ControllerBase
  {
    private readonly ITodoListService _todoListService;

    public TodoListsController(ITodoListService todoListService)
    {
      _todoListService = todoListService;
    }

    // GET: api/todolists
    [HttpGet]
    public async Task<ActionResult<IList<TodoListListDTO>>> GetTodoLists()
    {

      var todoLists = await _todoListService.GetAllAsync();

      return Ok(todoLists.Select(t=> new TodoListListDTO(t)).ToList());
    }

    // GET: api/todolists/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDTO>> GetTodoList(long id)
    {
      if (!await _todoListService.ExistsAsync(id))
      {
        return NotFound();
      }
      var todoList = await _todoListService.GetAsync(id);
      
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
      
      if (!await _todoListService.ExistsAsync(id))
      {
        return NotFound();
      }

      await _todoListService.UpdateAsync(id, todoList);

      return NoContent();
    }

    // POST: api/todolists
    // To protect from over-posting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<TodoList>> PostTodoList(CreateTodoListDTO todoListDto)
    {
      if (await _todoListService.ExistsAsync(todoListDto.Id))
      {
        return Problem($"TodoList with Id {todoListDto.Id} already exist", statusCode:(int)HttpStatusCode.Conflict);
      }

      TodoList todoList = todoListDto.ToEntity();
      await _todoListService.CreateAsync(todoList);

      return CreatedAtAction("GetTodoList", new { id = todoListDto.Id }, todoListDto);
    }

    // DELETE: api/todolists/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTodoList(long id)
    {
      if (!await _todoListService.ExistsAsync(id))
      {
        return NotFound();
      }
      
      await _todoListService.DeleteAsync(id);

      return NoContent();
    }
  }
}
