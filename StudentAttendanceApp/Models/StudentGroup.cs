using DocumentFormat.OpenXml.Spreadsheet;

namespace StudentAttendanceApp.Models
{
    public class StudentGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int TrackId { get; set; }
        public Track Track { get; set; }

        //public ICollection<User> Users { get; set; }
    }

}
