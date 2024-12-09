using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Tasks.EntityModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.VisualBasic;
namespace Tasks.Web.Pages;
public class ActivityDetailsModel : PageModel
{
    private TasksContext _db;


    public ActivityDetailsModel(TasksContext db)
    {
        _db = db;
    }

    public IEnumerable<ActivityDetail>? ActivityDetails { get; set; }


    public List<int> usedTasks { get; set; } = new();
    public List<string>? Suggestions { get; set; } = new();
    public IEnumerable<EntityModels.Task> tasks { get; set; }
    public IEnumerable<Person> Persons { get; set; }


    public ActivityDetail ActivityDetail { get; set; } = new();
    public EntityModels.Task task { get; set; } = new();

    public Activity activity{ get; set; }



    [BindProperty]
    public string SearchBox { get; set; }

    [BindProperty]
    public int index { get; set; }



    public void ActivityDetailsByID(int id)
    {
        ActivityDetails = _db.ActivityDetails.Where(a => a.ActivityID == id).Include(a => a.Activity).Include(a => a.Person).Include(a => a.Task).OrderBy(a => a.Index);
    }

    public void UsedTasks()
    {
        foreach (var a in ActivityDetails)
        {
            usedTasks.Add(a.TaskID);
        }
        tasks = _db.Tasks.Where(a => !usedTasks.Contains(a.TaskID));
    }

    public Boolean permitted(ActivityDetail activityDetail, Person person)
    {
        foreach (var t in person.TaskExceptions)
        {
            if (activityDetail.TaskID == t.TaskID) return false;
        }
        return true;
    }

    public void OnGet(int id)
    {
         activity=_db.Activities.First(a => a.ActivityID == id);
         ViewData["title"] = $"ActivityDetails/{activity.ActivityName}";


        ActivityDetailsByID(id);
        UsedTasks();
        TempData["id"] = id;
        TempData["n"] = ActivityDetails.Count();

        foreach (var a in tasks)
        {
            Suggestions!.Add(a.TaskName);
        }




    }

    public IActionResult OnPostDetails(int ActivityId)
    {
        WriteLine(ActivityId);
        return RedirectToPage("/ActivityDetails", new { id = ActivityId });
    }
    public IActionResult OnPostDelete()
    {
        int Id = Convert.ToInt32(TempData["id"]);
        ActivityDetailsByID(Id);
        UsedTasks();
        foreach (var a in ActivityDetails)
        {
            if (a.Index > index)
            {

                a.Index--;
                _db.ActivityDetails.Update(a);
            }
            else if (a.Index == index)
            {

                _db.ActivityDetails.Remove(a);
            }
        }


        _db.SaveChanges();
        return RedirectToPage("/ActivityDetails", new { id = Id });
    }

    public IActionResult OnPostAdd()
    {
        int Id = Convert.ToInt32(TempData["id"]);

        ActivityDetailsByID(Id);
        UsedTasks();

       

            task = _db.Tasks.Where(a => !usedTasks.Contains(a.TaskID)).FirstOrDefault(a => a.TaskName == SearchBox)!;
            if (task != null)
            {
                int index = Convert.ToInt32(TempData["n"]);
                ActivityDetail.ActivityID = Id;
                ActivityDetail.TaskID = task.TaskID;
                ActivityDetail.Index = index + 1;
                _db.Add(ActivityDetail);
                _db.SaveChanges();

                return RedirectToPage("/ActivityDetails", new { id = Id });
            }
        
        else
        {
            TempData["ErrorMessage"] = "Verifique los datos del formulario";
            return RedirectToPage("/ActivityDetails", new { id = Id });
        }
    }



    public IActionResult OnPostUpdateOrder([FromBody] List<int> order)
    {
        int Id = Convert.ToInt32(TempData["id"]);



        // Actualizar el orden en la base de datos
        var details = _db.ActivityDetails.Where(a => a.ActivityID == Id).ToList();

        for (int i = 0; i < order.Count; i++)
        {
            var detail = details.FirstOrDefault(e => e.TaskID == order[i]);
            if (detail != null)
            {
                detail.Index = i + 1; // Actualizar el índice
            }
        }

        _db.SaveChanges();
        return RedirectToPage("/ActivityDetails", new { id = Id });
    }
    public IActionResult OnPostAssignResponsibler()
    {

        int Id = Convert.ToInt32(TempData["id"]);
        int n = _db.Persons.Count();
        if (n != 0)
        {
            ActivityDetails = _db.ActivityDetails.Where(a => a.ActivityID == Id);

            List<int?> unavailablePerson = new();
            foreach (var a in ActivityDetails)
            {
                if (a.PersonID is not null) { unavailablePerson.Add(a.PersonID); }
            }

            Person? Person;
            Random random = new Random();


            ActivityDetails = ActivityDetails.Where(a => a.PersonID == null);
            Persons = _db.Persons.Include(a => a.TaskExceptions);

            foreach (var a in ActivityDetails)
            {

                foreach (var p in Persons)
                {
                    int randomIndex = random.Next(n);
                    Person = _db.Persons.Include(p => p.TaskExceptions).Skip(randomIndex).FirstOrDefault();
                    if (permitted(a, Person) && !unavailablePerson.Contains(Person.PersonID) )
                    {
                        unavailablePerson.Add(Person.PersonID);
                        a.PersonID = Person.PersonID;
                        }
                    

                }

            }
            _db.UpdateRange(ActivityDetails);
            _db.SaveChanges();
        }


        return RedirectToPage("/ActivityDetails", new { id = Id });
    }

}




