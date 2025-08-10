using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;

namespace StudentAttendanceApp.Controllers
{
    //[Authorize(Roles = "Admin,Teacher")]
    public class BranchesController : Controller
    {
        private readonly AppDbContext _context;

        public BranchesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var branches = _context.Branches.ToList();
            return View(branches);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Branch branch)
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
            _context.Branches.Add(branch);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
            //    }
            return View(branch);
        }

        public IActionResult Edit(int id)
        {
            var branch = _context.Branches.Find(id);
            if (branch == null) return NotFound();
            return View(branch);
        }

        [HttpPost]
        public IActionResult Edit(Branch branch)
        {
            //if (ModelState.IsValid)
            //{
                _context.Branches.Update(branch);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
     //       }
            return View(branch);
        }

        public IActionResult Delete(int id)
        {
            var branch = _context.Branches.Find(id);
            if (branch == null) return NotFound();
            return View(branch);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var branch = _context.Branches.Find(id);
            if (branch == null) return NotFound();

            _context.Branches.Remove(branch);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
