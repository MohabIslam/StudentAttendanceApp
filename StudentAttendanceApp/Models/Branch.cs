namespace StudentAttendanceApp.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Track> Tracks { get; set; }
        public ICollection<User> Users { get; set; }
    }
}
