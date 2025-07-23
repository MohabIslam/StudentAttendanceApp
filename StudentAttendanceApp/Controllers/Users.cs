using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;
using StudentAttendanceApp.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace StudentAttendanceApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        private readonly IPasswordHasher<User> _passwordHasher;

        public UsersController(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }


        public IActionResult Index()
        {
            var users = _context.Users
                                .Include(u => u.Role)
                                .Include(u => u.Branch)
                                .ToList();
            return View(users);
        }

        public IActionResult Create()
        {
            var viewModel = new UserFormViewModel
            {
                User = new User(),
                Roles = _context.Roles.ToList(),
                Branches = _context.Branches.ToList()
            };
            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Create(UserFormViewModel vm)
        {
            vm.Roles = _context.Roles.ToList();

            vm.Branches = _context.Branches.ToList();


            // 🟢 اربط الـ Role و Branch من قاعدة البيانات بناءً على الـ Id
            var role = _context.Roles.Find(vm.User.RoleId);
            var branch = _context.Branches.Find(vm.User.BranchId);

            if (role == null || branch == null)
            {
                ModelState.AddModelError("", "Invalid Role or Branch selected.");
                return View(vm);
            }

            vm.User.RoleId = role.Id;
            vm.User.BranchId = branch.Id;
            vm.User.Role =  role ;
            vm.User.Branch = branch;

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
            }

            //if (!ModelState.IsValid)
            //{
           //     return View(vm);
       //     }


            vm.User.PasswordHash = _passwordHasher.HashPassword(vm.User, vm.User.PasswordHash);

            _context.Users.Add(vm.User);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            var viewModel = new UserFormViewModel
            {
                User = user,
                Roles = _context.Roles.ToList(),
                Branches = _context.Branches.ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(int id, UserFormViewModel vm)
        {
            if (id != vm.User.Id) return NotFound();

            //if (!ModelState.IsValid)
            //{
            //    vm.Roles = _context.Roles.ToList();
            //    vm.Branches = _context.Branches.ToList();
            //    return View(vm);
            //}

            var role = _context.Roles.Find(vm.User.RoleId);
            var branch = _context.Branches.Find(vm.User.BranchId);

            if (role == null || branch == null)
            {
                ModelState.AddModelError("", "Invalid Role or Branch selected.");
                return View(vm);
            }

            vm.User.RoleId = role.Id;
            vm.User.BranchId = branch.Id;
            vm.User.Role = role;
            vm.User.Branch = branch;



            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            user.FullName = vm.User.FullName;
            user.Email = vm.User.Email;
            user.RoleId = vm.User.RoleId;
            user.BranchId = vm.User.BranchId;
            if (!string.IsNullOrEmpty(vm.User.PasswordHash))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, vm.User.PasswordHash);
            }
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var user = _context.Users
                               .Include(u => u.Role)
                               .Include(u => u.Branch)
                               .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }
    }
 
}
