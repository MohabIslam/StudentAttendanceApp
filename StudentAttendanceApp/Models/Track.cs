namespace StudentAttendanceApp.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        public ICollection<StudentGroup> Groups { get; set; }
    }

}
