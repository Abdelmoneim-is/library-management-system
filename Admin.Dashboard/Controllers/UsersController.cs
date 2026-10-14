using Admin.Dashboard.ViewModels;
using LibraryInfrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Dashboard.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 1. Index مع الفلترة بالـ Role والبحث النصي الذكي
        [HttpGet]
        public async Task<IActionResult> Index(string? searchQuery, string? roleName)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                usersQuery = usersQuery.Where(u => u.UserName!.Contains(searchQuery) ||
                                                   u.Email!.Contains(searchQuery) ||
                                                   (u.PhoneNumber != null && u.PhoneNumber.Contains(searchQuery)));
            }

            var users = await usersQuery.ToListAsync();
            var userListVm = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!string.IsNullOrEmpty(roleName) && !roles.Contains(roleName))
                {
                    continue;
                }

                userListVm.Add(new UserWithRolesViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber!,
                    Roles = roles.ToList()
                });
            }

            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            ViewBag.RolesList = new SelectList(allRoles, roleName);

            ViewData["CurrentSearch"] = searchQuery;
            ViewData["CurrentRole"] = roleName;

            return View(userListVm);
        }

        // 2. Create (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            ViewBag.RolesList = allRoles;
            return View();
        }

        // 3. Create (POST) - مُحدث لتعيين الـ DisplayName
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.RolesList = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
                return View(model);
            }

            var user = new AppUser
            {
                DisplayName = model.DisplayName.Trim(), // 👈 تعيين قيمة الـ DisplayName لتجنب خطأ الـ NULL
                UserName = model.UserName.Trim(),
                Email = model.Email.Trim(),
                PhoneNumber = model.PhoneNumber?.Trim(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.SelectedRole))
                {
                    await _userManager.AddToRoleAsync(user, model.SelectedRole);
                }

                TempData["Success"] = "User created successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewBag.RolesList = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userVm = new UserWithRolesViewModel
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Roles = roles.ToList()
            };

            return View(userVm);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoles(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRolesVm = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Roles = new List<RoleManageViewModel>()
            };

            foreach (var role in allRoles)
            {
                var roleVm = new RoleManageViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name!,
                    IsSelected = await _userManager.IsInRoleAsync(user, role.Name!)
                };
                userRolesVm.Roles.Add(roleVm);
            }

            return View(userRolesVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoles(UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.RemoveFromRolesAsync(user, userRoles);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Cannot remove user existing roles";
                return View(model);
            }

            var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName);
            result = await _userManager.AddToRolesAsync(user, selectedRoles);
            if (!result.Succeeded)
            {
                TempData["Error"] = "Cannot add selected roles to user";
                return View(model);
            }

            TempData["Success"] = "User roles updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            if (user.UserName == User.Identity?.Name)
            {
                TempData["Error"] = "You cannot delete your own active account!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete user.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}