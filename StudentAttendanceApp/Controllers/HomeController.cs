using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;
using System.Diagnostics;

namespace StudentAttendanceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger , AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            var totalStudents = _context.Students.Count();
            var totalUsers = _context.Users.Count();
            var totalBranches = _context.Branches.Count();
            var totalGroups = _context.StudentGroups.Count();

            ViewData["Students"] = totalStudents;
            ViewData["Users"] = totalUsers;
            ViewData["Branches"] = totalBranches;
            ViewData["Groups"] = totalGroups;

            return View(); // ✅ الحل
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
