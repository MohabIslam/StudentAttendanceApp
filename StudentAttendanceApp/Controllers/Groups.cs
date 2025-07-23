using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentAttendanceApp.Controllers
{
    public class GroupsController : Controller
    {
        private readonly AppDbContext _context;

        public GroupsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var groups = _context.StudentGroups
                                 .Include(g => g.Track) // 🛡️ حمل التراك المرتبط
                                 .ThenInclude(t => t.Branch) // 🛡️ وحمل الفرع المرتبط بالتراك
                                 .ToList();
            return View(groups);
        }

        public IActionResult Create()
        {
            ViewBag.Tracks = new SelectList(_context.Tracks.Include(t => t.Branch)
                                                           .ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentGroup group)
        {
            //if (ModelState.IsValid)
            //{
                _context.StudentGroups.Add(group);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
        //    }
            ViewBag.Tracks = new SelectList(_context.Tracks, "Id", "Name", group.TrackId);
            return View(group);
        }

        public IActionResult Edit(int id)
        {
            var group = _context.StudentGroups.Find(id);
            if (group == null) return NotFound();

            ViewBag.Tracks = new SelectList(_context.Tracks, "Id", "Name", group.TrackId);
            return View(group);
        }

        [HttpPost]
        public IActionResult Edit(StudentGroup group)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }

            //if (ModelState.IsValid)
            //{
            _context.StudentGroups.Update(group);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
            //      }
            ViewBag.Tracks = new SelectList(_context.Tracks, "Id", "Name", group.TrackId);
            return View(group);
        }

        public IActionResult Delete(int id)
        {
            var group = _context.StudentGroups
                .Include(g => g.Track)
                .ThenInclude(t => t.Branch) // 🛡️ تحميل الـ Branch
                .FirstOrDefault(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var group = _context.StudentGroups.Find(id);
            if (group == null) return NotFound();

            _context.StudentGroups.Remove(group);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
