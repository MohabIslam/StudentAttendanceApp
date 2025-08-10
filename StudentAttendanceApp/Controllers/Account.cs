using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StudentAttendanceApp.Data;
using StudentAttendanceApp.Models;
using StudentAttendanceApp.ViewModels;

namespace StudentAttendanceApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly AppDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            AppDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }


        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                BranchList = _context.Branches
                    .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                    .ToList(),
                RoleList = new List<SelectListItem>
        {
            new SelectListItem { Value = "Student", Text = "Student" }
        }
            };

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                model.RoleList.InsertRange(0, new List<SelectListItem>
        {
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "Instructor", Text = "Instructor" }
        });
            }

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // إعادة تحميل الـ Branches و Roles بعد Post
            model.BranchList = _context.Branches
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Name })
                .ToList();

            model.RoleList = new List<SelectListItem>
    {
        new SelectListItem { Value = "Student", Text = "Student" }
    };

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                model.RoleList.InsertRange(0, new List<SelectListItem>
        {
            new SelectListItem { Value = "Admin", Text = "Admin" },
            new SelectListItem { Value = "Instructor", Text = "Instructor" }
        });
            }
            else
            {
                // إزالة حقل SelectedRole من التحقق للمستخدم العادي
                ModelState.Remove("SelectedRole");
            }

            if (!ModelState.IsValid)
            {
         //       return View(model);
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                BranchId = model.BranchId,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // تحديد الدور
                string role = "Student";
                if (User.Identity.IsAuthenticated && User.IsInRole("Admin") && !string.IsNullOrEmpty(model.SelectedRole))
                {
                    role = model.SelectedRole;
                }

                // تأكد من وجود الدور
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRoleAsync(user, role);

                if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Users");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Dashboard", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
