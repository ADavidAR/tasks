using Microsoft.AspNetCore.Mvc.RazorPages; // To use PageModel.
using Microsoft.AspNetCore.Mvc; // To use [BindProperty], IActionResult.
using Tasks.EntityModels;

namespace Tasks.Web.Pages;

public class TasksModel : PageModel
{

  private TasksContext _db;

  public TasksModel(TasksContext db)
  {
    _db = db;
  }

  public IEnumerable<EntityModels.Task>? Tasks { get; set; }

  [BindProperty]
  public EntityModels.Task? Task { get; set; }

  public void OnGet()
  {
    ViewData["title"] = "Tareas";
    Tasks = _db.Tasks.OrderBy(t => t.TaskName.ToLower());
  }

  public IActionResult OnPostAdd()
  {
    if (Task is not null && ModelState.IsValid)
    {
      bool exists = _db.Tasks.Any(t => t.TaskName.ToLower() == Task.TaskName.ToLower());
      
      if (exists)
      {
        TempData["Exists"] = "La tarea ya existe. Por favor, ingrese un nombre diferente.";
        return RedirectToPage("Tasks");
      }
      _db.Tasks.Add(Task);
      _db.SaveChanges();
      return RedirectToPage("Tasks");
    }
    else
    {
      return BadRequest();
    }
  }

  public IActionResult OnPostDelete(int idToDelete)
  {
    if (IsIdInUse(idToDelete))
    {
      TempData["InUse"] = "Tarea en uso. No puede ser eliminada.";
      return RedirectToPage("Tasks");
    }

    var itemToRemove = _db.Tasks.FirstOrDefault(i => i.TaskID == idToDelete);
    if (itemToRemove != null)
    {
      _db.Tasks.Remove(itemToRemove);
      _db.SaveChanges();
    }

    return RedirectToPage("Tasks"); 
  }

  private bool IsIdInUse(int id)
  {
    if(_db.TaskExceptions.Any(te => te.TaskID == id) || _db.ActivityDetails.Any(ad => ad.TaskID == id)){
      return true; 
    }
    return false;

  } 
}