using System.ComponentModel.DataAnnotations;

namespace OfficeSuit.Models
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        [StringLength(200)]
        public string TaskName { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        [Display(Name = "Assigned To")]
        public string? AssignTo { get; set; }   // UserId from UserInfo

        [Required]
        public int? UserId { get; set; }

        [Required]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }
    }
}
