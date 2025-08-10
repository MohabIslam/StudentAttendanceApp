using Microsoft.AspNetCore.Mvc.Rendering;
using StudentAttendanceApp.Models;
using System.Collections.Generic;

namespace StudentAttendanceApp.ViewModels
{
    public class UserFormViewModel
    {
        public User User { get; set; }
        public List<SelectListItem> Branches { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public string SelectedRole { get; set; }
        public bool IsEditMode => User != null && !string.IsNullOrEmpty(User.Id);
    }
}
