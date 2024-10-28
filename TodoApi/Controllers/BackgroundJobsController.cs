using Hangfire;
using IServices;
using Microsoft.AspNetCore.Mvc;

namespace TodoApi.Controllers;

[Route("api/backgroundjobs")]
[ApiController]
public class BackgroundJobsController : ControllerBase
{
    private readonly IBackgroundJobClient _jobClient;
    private readonly ITodoItemService _todoItemService;
    private readonly IConfiguration _configuration;
    

    public BackgroundJobsController(IBackgroundJobClient jobClient, ITodoItemService todoItemService, 
        IConfiguration configuration)
    {
        _jobClient = jobClient;
        _todoItemService = todoItemService;
        _configuration = configuration;
    }
    
    // GET: api/backgroundjobs
    [HttpGet("getolditemsdeleted/{dateFrom}")]
    public async Task<ActionResult> GetOldItemsDeleted(DateTime dateFrom)
    {
        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var scheduledTime = _configuration["TodoAppConfig:ScheduledTime"];
        var scheduledDateAndTime = $"{today}T{scheduledTime}";
        
        _jobClient.Schedule(() => EnqueOldItemsDeletion(dateFrom),DateTimeOffset.Parse(scheduledDateAndTime));

        return NoContent();
    }

    [NonAction]
    public async Task EnqueOldItemsDeletion(DateTime dateFrom)
    {
        int rowsToDelete = Int32.Parse(_configuration["TodoAppConfig:RowsToDeletePerJob"]);
        var callAgain = await _todoItemService.CleanOldData(dateFrom, rowsToDelete);
        var total = (await _todoItemService.GetAllAsync()).Count(t => t.Created <= dateFrom);
        Console.WriteLine($"Remaining TodoItems: {total}");
        if (callAgain)
            _jobClient.Enqueue(() => EnqueOldItemsDeletion(dateFrom)
            );
    }

}