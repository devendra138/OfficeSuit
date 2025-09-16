using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OfficeSuit.Models
{
    public class UserInfo
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50, ErrorMessage = "First Name can't be longer than 50 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50, ErrorMessage = "Last Name can't be longer than 50 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Contact is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string Contact { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } // Could also be an enum

        [Required(ErrorMessage = "Designation is required")]
        public int DesignationId { get; set; }

        public int IsActive { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation: A Manager can manage many projects
        public ICollection<ProjectModel> Projects { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

    }
}
