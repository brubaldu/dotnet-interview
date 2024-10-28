using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Repository;
using Services;
using TodoApi.Controllers;

namespace TodoApi.Tests.Controllers;

public class BackgroundJobsControllerTests
{
    private DbContextOptions<TodoContext> DatabaseContextOptions()
    {
        return new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }
  
    private BackgroundJobsController CreateController(TodoContext context, IBackgroundJobClient backgroundJobClient,
        Dictionary<string,string> settings)
    {
        var repository = new TodoItemRepository(context);
        var todoItemService = new TodoItemService(repository);
        

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        return new BackgroundJobsController(backgroundJobClient, todoItemService, configuration);
    }
    
    [Fact]
    public async Task GetOldItemsDeleted_WhenCalled_IsScheduledAtCorrectTime()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            var client = new Mock<IBackgroundJobClient>();
            
            var inMemorySettings = new Dictionary<string, string> {
                {"TodoAppConfig:RowsToDeletePerJob", "10"},
                {"TodoAppConfig:ScheduledTime", "12:00:00-03:00"}
            };
            
            var controller = CreateController(context, client.Object, inMemorySettings);

            var dateFrom = new DateTime(2023, 01, 01);

            var result = await controller.GetOldItemsDeleted(dateFrom);
            
            Assert.IsType<NoContentResult>(result);

            var scheduledAt = DateTime.Today;
            // This is because we are in -3 GMT and it's scheduled in UTC
            scheduledAt = scheduledAt.AddHours(15);
      
            client.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "EnqueOldItemsDeletion" && 
                                  job.Args[0].ToString() == dateFrom.ToString()),
                It.Is<ScheduledState>(ss => Matches(ss.EnqueueAt, 
                    scheduledAt))), Times.Once);
            
        }
    }
    
    [Fact]
    public async Task GetOldItemsDeleted_WhenCalled_IsScheduledAtCorrectTime2()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            var client = new Mock<IBackgroundJobClient>();
            
            var inMemorySettings = new Dictionary<string, string> {
                {"TodoAppConfig:RowsToDeletePerJob", "10"},
                {"TodoAppConfig:ScheduledTime", "18:00:00-03:00"}
            };
            
            var controller = CreateController(context, client.Object, inMemorySettings);

            var dateFrom = new DateTime(2023, 01, 01);

            var result = await controller.GetOldItemsDeleted(dateFrom);
            
            Assert.IsType<NoContentResult>(result);

            var scheduledAt = DateTime.Today;
            // This is because we are in -3 GMT and it's scheduled in UTC
            scheduledAt = scheduledAt.AddHours(21);
      
            client.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "EnqueOldItemsDeletion" && job.Args[0].ToString() == dateFrom.ToString()),
                It.Is<ScheduledState>(ss => Matches(ss.EnqueueAt, scheduledAt))), Times.Once);
            
        }
    }
    
    public bool Matches(DateTime scheduledDate, DateTime expectedDate)
    {
        return scheduledDate == expectedDate;
    }
}