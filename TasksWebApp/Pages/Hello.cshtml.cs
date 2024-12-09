using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tasks.EntityModels;
namespace Tasks.Web.Pages;
public class HelloModel : PageModel
{
    public string Name = "Sho";

    public TasksContext _db;
    public Person p;

    public HelloModel(TasksContext db) 
    {
       using TasksContext d = new();
       db = d;
       _db = d;
        p = d.Persons.Single(p => p.PersonID == 1);
    }
}