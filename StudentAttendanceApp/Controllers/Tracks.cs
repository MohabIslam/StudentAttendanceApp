using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;

namespace StudentAttendanceApp.Controllers
{

    //[Authorize(Roles = "Admin,Teacher")]
    public class TracksController : Controller
    {
        private readonly AppDbContext _context;

        public TracksController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tracks = _context.Tracks
                                 .Select(t => new
                                 {
                                     t.Id,
                                     t.Name,
                                     t.CreatedAt,
                                     BranchName = t.Branch.Name != null ? t.Branch.Name : "N/A"
                                 })
                                 .ToList();
            ViewBag.Tracks = tracks;
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.Branches = new SelectList(_context.Branches, "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Track track)
        {
            //if (ModelState.IsValid)
            //{
                _context.Tracks.Add(track);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
    //        }
            ViewBag.Branches = new SelectList(_context.Branches, "Id", "Name", track.BranchId);
            return View(track);
        }

        public IActionResult Edit(int id)
        {
            var track = _context.Tracks.Find(id);
            if (track == null) return NotFound();

            ViewBag.Branches = new SelectList(_context.Branches, "Id", "Name", track.BranchId);
            return View(track);
        }

        [HttpPost]
        public IActionResult Edit(Track track)
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
            _context.Tracks.Update(track);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
            //     }
            ViewBag.Branches = new SelectList(_context.Branches, "Id", "Name", track.BranchId);
            return View(track);
        }

        public IActionResult Delete(int id)
        {
            var track = _context.Tracks
                    .Include(t => t.Branch) //  تحميل الفرع المرتبط
                    .FirstOrDefault(t => t.Id == id);
            if (track == null) return NotFound();
            return View(track);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var track = _context.Tracks
                    .Include(t => t.Branch) //  تحميل الفرع المرتبط
                    .FirstOrDefault(t => t.Id == id);
            if (track == null) return NotFound();

            _context.Tracks.Remove(track);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
