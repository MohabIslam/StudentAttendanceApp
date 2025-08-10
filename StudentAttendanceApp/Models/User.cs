using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceApp.Models
{
    public class User : IdentityUser
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign Key to Branch
        [Required(ErrorMessage = "Branch is required")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}
    