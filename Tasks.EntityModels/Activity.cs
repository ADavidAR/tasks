using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tasks.EntityModels;

[Index("Date", Name = "Date")]

public class Activity
{
    [Key]
    [Column("ActivityID", TypeName = "integer")]
    public int ActivityID { set; get; }

    
    [Column("ActivityName", TypeName = "nvarchar (50)")]
    [StringLength(50)]
    [Required]
    public string ActivityName { set; get; } = null!;

    [Column("Date", TypeName = "date")]
    [Required]
    public DateTime Date { set; get; }

    [Column("Description", TypeName = "nvarchar (100)")]
    [StringLength(100)]
    public string Description { set; get; } = null!;

    [InverseProperty("Activity")]
    public virtual ICollection<ActivityDetail> ActivityDetails { set; get; } = new List<ActivityDetail>();
} 