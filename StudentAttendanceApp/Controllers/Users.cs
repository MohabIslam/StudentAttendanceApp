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
            var users = await _userManager.Users
                .Include(u => u.Branch)
                .AsNoTracking()
                .ToListAsync();

            var userRoles = await (from ur in _context.UserRoles
                                   join r in _context.Roles on ur.RoleId equals r.Id
                                   select new { ur.UserId, RoleName = r.Name })
                                  .ToListAsync();

            var userList = users.Select(u => new UserWithRoleViewModel
            {
                User = u,
                RoleName = userRoles.FirstOrDefault(r => r.UserId == u.Id)?.RoleName ?? "No Role"
            }).ToList();

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
            //return View(viewModel);
            return View("Create", viewModel);

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

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRole = userRoles.FirstOrDefault();

            var viewModel = new UserFormViewModel
            {
                User = user,
                Branches = _context.Branches
                    .Select(b => new SelectListItem
                    {
                        Value = b.Id.ToString(),
                        Text = b.Name,
                        Selected = user.BranchId == b.Id
                    }).ToList(),
                Roles = _roleManager.Roles
                    .Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name,
                        Selected = r.Name == selectedRole
                    }).ToList(),
                SelectedRole = selectedRole
            };

            //return View(viewModel);
            return View("Edit", viewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, UserFormViewModel model, string NewPassword)
        {
            if (id != model.User.Id)
                return NotFound();

            //if (!ModelState.IsValid)
            //{
            //    model.Branches = _context.Branches
            //        .Select(b => new SelectListItem
            //        {
            //            Value = b.Id.ToString(),
            //            Text = b.Name,
            //            Selected = b.Id == model.User.BranchId
            //        }).ToList();

            //    model.Roles = _roleManager.Roles
            //        .Select(r => new SelectListItem
            //        {
            //            Value = r.Name,
            //            Text = r.Name,
            //            Selected = r.Name == model.SelectedRole
            //        }).ToList();

            //    return View(model);
            //}

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            // Update basic info
            user.FullName = model.User.FullName;
            user.Email = model.User.Email;
            user.UserName = model.User.Email;
            user.BranchId = model.User.BranchId;

            // Update password if provided
            if (!string.IsNullOrWhiteSpace(NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, NewPassword);
                if (!passResult.Succeeded)
                {
                    foreach (var error in passResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    // Repopulate dropdowns and return view
                    model.Branches = _context.Branches
                        .Select(b => new SelectListItem
                        {
                            Value = b.Id.ToString(),
                            Text = b.Name,
                            Selected = b.Id == model.User.BranchId
                        }).ToList();

                    model.Roles = _roleManager.Roles
                        .Select(r => new SelectListItem
                        {
                            Value = r.Name,
                            Text = r.Name,
                            Selected = r.Name == model.SelectedRole
                        }).ToList();

                    return View(model);
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            // Update role
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(model.SelectedRole) && await _roleManager.RoleExistsAsync(model.SelectedRole))
                await _userManager.AddToRoleAsync(user, model.SelectedRole);

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
