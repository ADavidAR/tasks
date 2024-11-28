using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tasks.EntityModels;

[PrimaryKey( "PersonID", "TaskID")]
[Table("ActivityDetails")]
[Index("TaskID", Name = "TaskID")]
[Index("TaskID", Name = "TasksActivityDetails")]
[Index("ActivityID", Name = "ActivityID")]
[Index("ActivityID", Name = "ActivitiesActivityDetails")]
[Index("PersonID", Name = "PersonID")]
[Index("PersonID", Name = "PersonsActivityDetails")]
[Index("Index", Name = "orderIndex")]
public class ActivityDetail
{
    [Key]
    [Column("TaskID", TypeName = "integer")]
    public int TaskID { set; get; }

    [Key]
    [Column("PersonID", TypeName = "integer")]
    public int? PersonID { set; get; }
    
    [Key]
    [Column("ActivityID", TypeName = "integer")]
    public int ActivityID { set; get; }

    [Column("OrderIndex", TypeName = "integer")]
    public int Index { set; get; }

    [ForeignKey("ActivityID")]
    [InverseProperty("ActivityDetails")]
    public virtual Activity Activity { set; get; } = null!;

    [ForeignKey("TaskID")]
    [InverseProperty("ActivityDetails")]
    public virtual Task Task { set; get; } = null!;

    [ForeignKey("PersonID")]
    [InverseProperty("ActivityDetails")]
    public virtual Person Person { set; get; } = null!;
}