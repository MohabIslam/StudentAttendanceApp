using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Helpers;
using StudentAttendanceApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace StudentAttendanceApp.Controllers
{
    //[Authorize]
    public class AttendancesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AttendancesController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        #region Index
        // GET: Attendances
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Attendances.Include(a => a.Student);
            return View(await appDbContext.ToListAsync());
        }
        #endregion

        #region Details
        // GET: Attendances/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        #endregion

        #region Create
        // GET: Attendances/Create
        public IActionResult Create()
        {
            ViewBag.Students = _context.Students.ToList();
            return View();
        }

        // POST: Attendances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudentId,Date,Status")] Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", attendance.StudentId);
            return View(attendance);
        }

        #endregion

        #region Edit

        // GET: Attendances/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load attendance with related student info
            var attendance = await _context.Attendances
                                           .Include(a => a.Student)
                                           .FirstOrDefaultAsync(a => a.Id == id);
            if (attendance == null)
            {
                return NotFound();
            }

            // No need for ViewData dropdown anymore because we show Student as readonly
            return View(attendance);
        }

        // POST: Attendances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,Date,Status")] Attendance attendance)
        {
            if (id != attendance.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendance);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Attendances.Any(a => a.Id == attendance.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(attendance);
        }

        #endregion

        #region Delete
        // GET: Attendances/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendance = await _context.Attendances
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendance == null)
            {
                return NotFound();
            }

            return View(attendance);
        }

        // POST: Attendances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceExists(int id)
        {
            return _context.Attendances.Any(e => e.Id == id);
        }


        #endregion

        #region Mark Attendance
        public IActionResult MarkAttendance()
        {
            var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            var allowedIps = _configuration.GetSection("AllowedIPs").Get<List<string>>();

            if (!IpHelper.IsIpAllowed(clientIp, allowedIps))
            {
                return Content("❌ You must be connected to the classroom Wi-Fi to mark attendance.");
            }

            var students = _context.Students.ToList();
            return View(students);
        }


        [HttpGet]
        public IActionResult MarkPresent()
        {
            ViewBag.Students = _context.Students.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult MarkPresent(int studentId)
        {


            if (!_context.Students.Any(s => s.Id == studentId))
            {
                TempData["Error"] = "Invalid Student ID!";
                return View("~/Views/Shared/ErrorPage.cshtml");
            }
            string userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
            var allowedRanges = new List<string>
                {
                    "192.168.1.0/24", // Example: Campus WiFi range
                    "10.0.0.0/16"// Add your real campus IP ranges
                };

            if (!IpHelper.IsIpAllowed(userIp, allowedRanges))
            {
                TempData["Error"] = "❌ You are not on the authorized campus network.";
                return View("~/Views/Shared/ErrorPage.cshtml");
            }

            var attendance = new Attendance
            {
                StudentId = studentId,
                Date = DateTime.Now,
                Status = "Present"
            };

            _context.Attendances.Add(attendance);
            _context.SaveChanges();

            TempData["Success"] = "✅ Your attendance has been marked!";
            return View("~/Views/Shared/SuccessPage.cshtml");
        } 
        #endregion

        #region ExportToExcel
        public IActionResult ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Attendance");

                // Add header
                worksheet.Cell(1, 1).Value = "Student Name";
                worksheet.Cell(1, 2).Value = "Date";
                worksheet.Cell(1, 3).Value = "Status";

                // Add data
                var attendanceList = _context.Attendances
                    .Select(a => new
                    {
                        StudentName = a.Student.Name,
                        a.Date,
                        a.Status
                    }).ToList();

                int row = 2;
                foreach (var record in attendanceList)
                {
                    worksheet.Cell(row, 1).Value = record.StudentName;
                    worksheet.Cell(row, 2).Value = record.Date.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 3).Value = record.Status;
                    row++;
                }

                // Style header
                var headerRange = worksheet.Range("A1:C1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.Crimson;
                headerRange.Style.Font.FontColor = XLColor.White;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Attendance.xlsx");
                }
            }
        }

        #endregion
    }
}
