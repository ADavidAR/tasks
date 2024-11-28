using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tasks.EntityModels;

[PrimaryKey("PersonID", "TaskID")]
[Table("TaskExceptions")]
[Index("PersonID", Name = "PersonID")]
[Index("PersonID", Name = "PersonsTaskExceptions")]
[Index("TaskID", Name = "TaskID")]
[Index("TaskID", Name = "TasksTaskExceptions")]
public class TaskException
{
    [Key]
    [Column("TaskID", TypeName = "integer")]
    public int TaskID { set; get; }

    [Key]
    [Column("PersonID", TypeName = "integer")]
    public int PersonID { set; get; }

    [ForeignKey("TaskID")]
    [InverseProperty("TaskExceptions")]
    public virtual Task Task { set; get; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("TaskExceptions")]
    public virtual Person Person { set; get; } = null!;
}