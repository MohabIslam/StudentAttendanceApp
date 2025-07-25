﻿namespace StudentAttendanceApp.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } // Present/Absent
    }

}
