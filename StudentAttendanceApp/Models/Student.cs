namespace StudentAttendanceApp.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Class { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
    }

}
