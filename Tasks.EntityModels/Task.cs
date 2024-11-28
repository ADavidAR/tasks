using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tasks.EntityModels;

[Index("TaskID", Name = "TaskID")]
[Index("TaskName", Name = "TaskName")]
public class Task
{
    [Key]
    [Column("TaskID", TypeName = "integer")]
    public int TaskID { set; get; }

    
    [Column("TaskName", TypeName = "nvarchar (50)")]
    [StringLength(50)]
    [Required]
    public string TaskName { set; get; } = null!;

    [Column("Description", TypeName = "nvarchar (100)")]
    [StringLength(100)]
    public string Description { set; get; } = null!;

    [InverseProperty("Task")]
    public virtual ICollection<ActivityDetail> ActivityDetails { set; get; } = new List<ActivityDetail>();

    [InverseProperty("Task")]
    public virtual ICollection<TaskException> TaskExceptions { set; get; } = new List<TaskException>();

} 