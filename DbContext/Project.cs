using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeSuit.Models
{
    [Table("Project")]
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProjectName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; } // nullable in case project is ongoing

        [Required]
        [Display(Name = "Project Manager")]
        public int ProjectManagerID { get; set; }

        [ForeignKey("ProjectManagerID")]
        public UserInfo Manager { get; set; }   // 👈 references UserInfo
    }
}
