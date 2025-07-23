using StudentAttendanceApp.Models;

namespace StudentAttendanceApp.ViewModels
{
    public class UserFormViewModel
    {
        public User User { get; set; }
        public List<Role> Roles { get; set; }
        public List<Branch> Branches { get; set; }
        public bool IsEditMode => User.Id != 0;
    }
}
