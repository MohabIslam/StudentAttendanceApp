using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;
using StudentAttendanceApp.ViewModels;

namespace StudentAttendanceApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Include(u => u.Branch).ToListAsync();

            var userList = new List<UserWithRoleViewModel>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserWithRoleViewModel
                {
                    User = user,
                    RoleName = roles.FirstOrDefault() ?? "No Role"
                });
            }

            return View(userList);
        }

        public IActionResult Create()
        {
            var viewModel = new UserFormViewModel
            {
                User = new User(),
                Branches = _context.Branches.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList(),
                Roles = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserFormViewModel vm)
        {
            vm.Branches = _context.Branches.Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name
            }).ToList();

            vm.Roles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();

            if (!ModelState.IsValid)
                return View(vm);

            var user = new User
            {
                UserName = vm.User.Email,
                Email = vm.User.Email,
                FullName = vm.User.FullName,
                BranchId = vm.User.BranchId,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, vm.User.PasswordHash);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(vm);
            }

            if (!string.IsNullOrEmpty(vm.SelectedRole))
                await _userManager.AddToRoleAsync(user, vm.SelectedRole);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);

            var viewModel = new UserFormViewModel
            {
                User = user,
                Branches = _context.Branches.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Name
                }).ToList(),
                Roles = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList(),
                SelectedRole = currentRoles.FirstOrDefault()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserFormViewModel vm)
        {
            if (id != vm.User.Id) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = vm.User.FullName;
            user.Email = vm.User.Email;
            user.UserName = vm.User.Email;
            user.BranchId = vm.User.BranchId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Update failed");
                return View(vm);
            }

            var oldRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, oldRoles);

            if (!string.IsNullOrEmpty(vm.SelectedRole))
                await _userManager.AddToRoleAsync(user, vm.SelectedRole);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.Users
                .Include(u => u.Branch)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new UserDeleteViewModel
            {
                User = user,
                RoleName = roles.FirstOrDefault() ?? "No Role"
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
