using Microsoft.EntityFrameworkCore;
using Tasks.EntityModels;

using TasksContext db = new();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "tasks.db");

string dir = Environment.CurrentDirectory;


if (dir.EndsWith("wwwroot"))
{
    // En el directorio del proyecto.
    dbPath = Path.Combine(dir,"tasks.db");
}

builder.Services.AddDbContext<TasksContext>();

var app = builder.Build();

if(!app.Environment.IsDevelopment()) {
 app.UseHsts();
 Console.WriteLine("Modo Produccion");
}
 app.UseHttpsRedirection();
app.UseDefaultFiles();
 app.UseStaticFiles();
 app.MapRazorPages();
Tasks.EntityModels.Task ta =db.Tasks.Single(t => t.TaskID == 1);
app.MapGet("/hola", () => "Hola Mundo!"+ ta.TaskName);

app.Run();
Console.WriteLine("Finalizando ejecución");