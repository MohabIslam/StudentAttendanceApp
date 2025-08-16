using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;
using StudentAttendanceApp.ViewModels;

namespace StudentAttendanceApp.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel();

            // المستخدم الحالي
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            // الدور الحالي
            var roles = await _userManager.GetRolesAsync(currentUser);
            vm.CurrentRole = roles.FirstOrDefault() ?? "Student";

            // IDs للأدوار
            var roleIdsDict = await _context.Roles
                .Where(r => r.Name == "Student" || r.Name == "Instructor")
                .ToDictionaryAsync(r => r.Name, r => r.Id);

            var studentRoleId = roleIdsDict.ContainsKey("Student") ? roleIdsDict["Student"] : null;
            var instructorRoleId = roleIdsDict.ContainsKey("Instructor") ? roleIdsDict["Instructor"] : null;

            // حسابات عامة (Admin)
            if (roles.Contains("Admin"))
            {
                // حساب عدد الطلاب
                int studentsCount = 0;
                if (studentRoleId != null)
                    studentsCount = await _context.UserRoles.CountAsync(ur => ur.RoleId == studentRoleId);

                // حساب عدد المدرسين
                int instructorsCount = 0;
                if (instructorRoleId != null)
                    instructorsCount = await _context.UserRoles.CountAsync(ur => ur.RoleId == instructorRoleId);

                // عدد الفروع والمجموعات
                int branchesCount = await _context.Branches.CountAsync();
                int groupsCount = await _context.StudentGroups.CountAsync();

                // Attendance اليوم
                var today = DateTime.Today;
                var todayStart = today;
                var todayEnd = today.AddDays(1);

                var todayAttendanceQuery = _context.Attendances
                    .Where(a => a.Date >= todayStart && a.Date < todayEnd);

                int todayTotal = await todayAttendanceQuery.CountAsync();
                int todayPresent = await todayAttendanceQuery.CountAsync(a => a.Status == "Present");
                int todayAbsent = await todayAttendanceQuery.CountAsync(a => a.Status == "Absent");

                // آخر 5 مستخدمين
                var recentUsers = await _context.Users
                    .OrderByDescending(u => u.Id)
                    .Take(5)
                    .AsNoTracking()
                    .ToListAsync();

                // تعبئة الـ ViewModel
                vm.StudentsCount = studentsCount;
                vm.InstructorsCount = instructorsCount;
                vm.BranchesCount = branchesCount;
                vm.GroupsCount = groupsCount;
                vm.TodayTotal = todayTotal;
                vm.TodayPresent = todayPresent;
                vm.TodayAbsent = todayAbsent;
                vm.RecentUsers = recentUsers;
            }

            return View(vm);
        }
    }
}
