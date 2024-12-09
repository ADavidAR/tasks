using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Tasks.EntityModels;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Tasks.Web.Pages;

public class PersonsModel : PageModel
{
    public class RequestData {
        public required List<string> data { get; set; }
    }
    private TasksContext _db;

    public PersonsModel(TasksContext db)
    {
        _db = db;
    }

    public IEnumerable<Person>? Persons { set; get; }

    [BindProperty]
    public Person? Person { set; get; }

    public void OnGet() 
    {
        ViewData["title"] = "Personas";
        Persons = _db.Persons.Include(p => p.ActivityDetails).OrderBy(x => x.FirstName + x.LastName);
    }

    public IActionResult OnPostAddPerson()
    {
        if(Person is not null && ModelState.IsValid )
        {
            _db.Persons.Add(Person);
            _db.SaveChanges();
            return RedirectToPage("/Persons");
        } else {
            TempData["AddPersonError"] = "Verifique los datos del formulario";
            return Page();
        }
    }

    public IActionResult OnPostDeletePerson(int id)
    {
        Person p = _db.Persons.Include(p => p.TaskExceptions).Single(p => p.PersonID == id);
        _db.Persons.Remove(p);
        _db.SaveChanges();
        
        return RedirectToPage("/Persons");
    }

    public IActionResult OnPostAvailTasks([FromBody] RequestData Data)
    {
        if(!int.TryParse(Data.data[0], out int PersonId)){
            WriteLine("[ERROR]: No se pudo convetir '{1}' a int", Data.data[0]);
            return new JsonResult( new {success = false, data = ""} );
        }

        IEnumerable<TaskException> UsedTasks = _db.TaskExceptions.Where(te => te.PersonID == PersonId).Include(te => te.Task).OrderBy(te => te.Task.TaskName);

        List<int> UsedTasksIds = new();

        List<Object>[] nameIdPairs = new List<object>[2];

         nameIdPairs[0] = new();
         nameIdPairs[1] = new();

         foreach( var te in UsedTasks)
         {
            nameIdPairs[1].Add(new { name = te.Task.TaskName , id = te.TaskID });
            UsedTasksIds.Add(te.TaskID);
         }

        IEnumerable<Tasks.EntityModels.Task> AvailTasks = _db.Tasks.Where(t => !UsedTasksIds.Contains(t.TaskID)).OrderBy(t => t.TaskName);

         foreach(var t in AvailTasks)
         {
            nameIdPairs[0].Add(new { name = t.TaskName, id = t.TaskID});
         }

         return new JsonResult( new { success = true, data = nameIdPairs });
    }

    public IActionResult OnPostAddException(int PersonID, int TaskID)
    {

        WriteLine("En Add");
             
        TaskException te = new()
        {
            TaskID = TaskID,
            PersonID = PersonID
        };

        _db.TaskExceptions.Add(te);
        _db.SaveChanges();
        
        return RedirectToPage("/Persons");
    }

    public IActionResult OnPostDeleteException(int PersonID, int TaskID)
    {
        TaskException te = _db.TaskExceptions.Single(te => te.PersonID == PersonID && te.TaskID == TaskID);
        WriteLine("Antes de borrar");
        _db.TaskExceptions.Remove(te);
        WriteLine("Despues de borrar");
        _db.SaveChanges();
        WriteLine("Despues de guardar");
        
        return RedirectToPage("/Persons");
    }
    public string getPersonExceptionTasksNames(int id)
    {

        IQueryable<TaskException> TaskExceptions = _db.TaskExceptions.Include(te => te.Task).Where( te => te.PersonID == id);
        string tasksNames = "";
        foreach(TaskException te in TaskExceptions)
        {
            tasksNames += te.Task.TaskName + ", "; 
        }

        if(String.IsNullOrEmpty(tasksNames))
            return "";
        
        return tasksNames.Substring(0, tasksNames.Length - 2);
    }

    
}