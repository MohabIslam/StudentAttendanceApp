﻿namespace StudentAttendanceApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // علاقة مع الـ Role
        public int RoleId { get; set; }
        public Role Role { get; set; }

        // علاقة مع الـ Branch
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }

}
