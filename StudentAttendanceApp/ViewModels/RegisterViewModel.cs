using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StudentAttendanceApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int BranchId { get; set; }

        public IEnumerable<SelectListItem> BranchList { get; set; }

        // For Admin registration only
        public string? SelectedRole { get; set; }
        public List<SelectListItem> RoleList { get; set; } = new();
    }
}
