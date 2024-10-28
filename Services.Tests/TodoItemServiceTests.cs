using Domain;
using Moq;

namespace Services.Tests;

public class TodoItemServiceTests
{
    [Fact]
    public async Task CleanOldData_WhenCalled_ParametersAreCorrect()
    {
        var repository = new Mock<IRepository.IRepository<TodoItem>>();
        var service = new TodoItemService(repository.Object);

        var date = new DateTime(2023, 01, 01);
        var rowsToDelete = 10;

        await service.CleanOldData(date, rowsToDelete);

        repository.Verify(r => r.CleanOldData(date, rowsToDelete), Times.Exactly(1));
    }
}