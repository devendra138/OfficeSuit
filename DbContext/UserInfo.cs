using OfficeSuit.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YourNamespace.Models
{
    [Table("UserInfo")]
    public class UserInfo
    {
        [Key]
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; }

        [Required, StringLength(50)]
        public string LastName { get; set; }

        [Required, StringLength(100)]
        public string Email { get; set; }

        public string Password { get; set; }
        public string Contact { get; set; }
        public string Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int DesignationId { get; set; }
        public string PasswordEncrypted { get; set; }


    }
}
