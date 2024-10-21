using Domain;
using Hangfire;
using IRepository;
using IServices;
using Microsoft.EntityFrameworkCore;
using Repository;
using Services;
using TodoApi.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddDbContext<TodoContext>(
        // Use SQL Server
        // opt.UseSqlServer(builder.Configuration.GetConnectionString("TodoContext"));
        opt => opt.UseInMemoryDatabase("TodoList")
    )
    .AddEndpointsApiExplorer()
    .AddControllers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1",
        new(){Title = "TodoList API", Version = "v1"});
});

builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddScoped<IRepository<TodoList>,TodoListRepository>();
builder.Services.AddScoped<IRepository<TodoItem>,TodoItemRepository>();
builder.Services.AddScoped<ITodoListService,TodoListService>();
builder.Services.AddScoped<ITodoItemService,TodoItemService>();

builder.Services.AddHangfire(x =>
    x.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage()
    );

builder.Services.AddHangfireServer();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseHangfireDashboard();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



var scope = app.Services.CreateScope();

using (TodoContext context = scope.ServiceProvider.GetRequiredService<TodoContext>())
{
    context.TodoLists.Add(new TodoList
        { Id = 1, Name = "List 1", Items = new List<TodoItem>() { new TodoItem { Id = 1, Text = "Text 1" } } });
    context.TodoLists.Add(new TodoList { Id = 2, Name = "List 2" });
    var todoList3 = new TodoList { Id = 3, Name = "List 3" };
    
    // Adding lot of TodoItems for testing background delete
    for (int i = 2; i < 1000; i++)
    {
        var todoItem = new TodoItem { Id = i, Text = $"Text {i}", Created = new DateTime(2023,01,01)};
        todoList3.Items.Add(todoItem);
    }
    context.TodoLists.Add(todoList3);
    
    var todoList4 = new TodoList { Id = 4, Name = "List 4" };
    for (int i = 1; i < 1000; i++)
    {
        var id = 1000 + i;
        var todoItem = new TodoItem { Id = id, Text = $"Text {id}", Created = new DateTime(2024,01,02)};
        todoList4.Items.Add(todoItem);
    }
    context.TodoLists.Add(todoList4);
    context.SaveChanges();
}

app.UseAuthorization();
app.MapControllers();
app.Run();