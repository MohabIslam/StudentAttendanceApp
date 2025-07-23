using Microsoft.AspNetCore.Mvc;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;

namespace StudentAttendanceApp.Controllers
{
    public class RolesController : Controller
    {
        private readonly AppDbContext _context;

        public RolesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Role role)
        {
            //if (ModelState.IsValid)
            //{
                _context.Roles.Add(role);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
      //      }
            return View(role);
        }

        public IActionResult Edit(int id)
        {
            var role = _context.Roles.Find(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost]
        public IActionResult Edit(Role role)
        {
            //if (ModelState.IsValid)
            //{
                _context.Roles.Update(role);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
     //       }
            return View(role);
        }

        public IActionResult Delete(int id)
        {
            var role = _context.Roles.Find(id);
            if (role == null) return NotFound();
            return View(role);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var role = _context.Roles.Find(id);
            if (role == null) return NotFound();

            _context.Roles.Remove(role);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
