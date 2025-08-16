using System;
using System.Collections.Generic;
using StudentAttendanceApp.Models;

namespace StudentAttendanceApp.ViewModels
{
    public class DashboardViewModel
    {
        // Role of current user
        public string CurrentRole { get; set; }

        // Admin stats
        public int StudentsCount { get; set; }
        public int InstructorsCount { get; set; }
        public int BranchesCount { get; set; }
        public int GroupsCount { get; set; }

        // Today attendance
        public int TodayTotal { get; set; }
        public int TodayPresent { get; set; }
        public int TodayAbsent { get; set; }

        // Lists
        public List<User> RecentUsers { get; set; } = new();
    }
}
