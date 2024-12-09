
using Microsoft.EntityFrameworkCore;

namespace Tasks.EntityModels;

public partial class TasksContext : DbContext
{
    public virtual DbSet<Activity> Activities { set; get; }

    public virtual DbSet<Person> Persons { set; get; }

    public virtual DbSet<Task> Tasks { set; get; }

    public virtual DbSet<TaskException> TaskExceptions { set; get; }

    public virtual DbSet<ActivityDetail> ActivityDetails { set; get; }

    public TasksContext()
    {
    }

    public TasksContext( DbContextOptions<TasksContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
        {
            string database = "db\\tasks.db";
            string dir = Environment.CurrentDirectory;
            string path = String.Empty;

            if (dir.EndsWith("net8.0"))
            {
                // En el directorio <proyecto>\bin\<Debug|Release>\net8.0.
                path = Path.Combine("..", "..", "..", "..", database);
            }
            else if (dir.EndsWith("wwwroot"))
            {
                path = Path.Combine(dir, "tasks.db");

            }
            else
            {
                // En el directorio del proyecto.
                path = Path.Combine("..", database);
            }

            path = Path.GetFullPath(path);

            try
            {
                TasksContextLogger.WriteLine($"Ruta a Base de Datos: {path}");
            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }

            if(!File.Exists(path))
            {
                throw new FileNotFoundException(
                    message: $"{path} no encontrado.", fileName: path);
            }

            optionsBuilder.UseSqlite($"Data Source={path}");

            optionsBuilder.LogTo(TasksContextLogger.WriteLine,
                new[] { Microsoft.EntityFrameworkCore
                    .Diagnostics.RelationalEventId.CommandExecuting });
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>( entity =>
        {
            entity.Property(e => e.Description).HasDefaultValue("");
        });

        modelBuilder.Entity<Task>( entity =>
        {
            entity.Property( e => e.Description).HasDefaultValue("");
        });

        modelBuilder.Entity<ActivityDetail>( entity =>
        {
            entity.HasOne(d => d.Person)
                .WithMany( p => p.ActivityDetails)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Task)
                .WithMany( p => p.ActivityDetails)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Activity)
                .WithMany( p => p.ActivityDetails)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TaskException>( entity =>
        {
            entity.HasOne(d => d.Task)
                .WithMany( p => p.TaskExceptions)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(te => te.Person)
                .WithMany(p => p.TaskExceptions)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        
    }
}
