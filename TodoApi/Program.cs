using Domain;
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

var app = builder.Build();

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
    context.SaveChanges();
}

app.UseAuthorization();
app.MapControllers();
app.Run();