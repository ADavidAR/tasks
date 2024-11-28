using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tasks.EntityModels;

[Index("FirstName", Name = "FirstName")]
[Index("LastName", Name = "LastName")]
[Index("PersonID", Name = "PersonID")]
public class Person
{
    [Key]
    [Column("PersonID", TypeName = "integer")]
    public int PersonID {get; set;}
    
    [Column("FirstName", TypeName = "nvarchar (50)")]
    [StringLength(50)]
    [Required]
    public string FirstName {get; set;} = "";

    [Column("LastName", TypeName = "nvarchar (50)")]
    [StringLength(50)]
    [Required]
    public string LastName {get; set;} = "";

    public string FullName 
    {
        get { return $"{FirstName} {LastName}"; }
    }

    [InverseProperty("Person")]
    public virtual ICollection<ActivityDetail> ActivityDetails { set; get; } = new List<ActivityDetail>();

    [InverseProperty("Person")]
    public virtual ICollection<TaskException> TaskExceptions { set; get; } = new List<TaskException>();
}