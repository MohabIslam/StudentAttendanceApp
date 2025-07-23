using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "Password is required")]
        public string PasswordHash { get; set; }

        // Foreign Keys
        [Required(ErrorMessage = "Role is required")]
        public int RoleId { get; set; }
        public Role Role { get; set; }

        [Required(ErrorMessage = "Branch is required")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}
