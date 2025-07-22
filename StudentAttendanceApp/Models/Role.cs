namespace StudentAttendanceApp.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // Admin, Instructor, Student

        public ICollection<User> Users { get; set; }
    }

}
