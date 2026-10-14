using Admin.Dashboard.ViewModels;
using LibraryInfrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Dashboard.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        private async Task<List<RoleViewModel>> GetRoleListAsync(string? searchQuery = null)
        {
            var query = _roleManager.Roles.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(r => r.Name != null && r.Name.Contains(searchQuery));
            }

            var roles = await query.ToListAsync();
            var roleViewModels = new List<RoleViewModel>();

            foreach (var role in roles)
            {
                var userCount = (await _userManager.GetUsersInRoleAsync(role.Name!)).Count;
                roleViewModels.Add(new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name!,
                    UserCount = userCount
                });
            }
            return roleViewModels;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchQuery)
        {
            var viewModel = new RolesIndexViewModel
            {
                Roles = await GetRoleListAsync(searchQuery),
                NewRole = new RoleViewModel()
            };

            ViewData["CurrentSearch"] = searchQuery;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolesIndexViewModel model)
        {
            // تنظيف أي Validation مالهاش لازمة للـ RoleId أو الخصائص التانية
            ModelState.ClearValidationState(nameof(model.NewRole.RoleId));

            if (string.IsNullOrWhiteSpace(model.NewRole?.RoleName))
            {
                ModelState.AddModelError("NewRole.RoleName", "Role Name Is Required");
            }

            if (!ModelState.IsValid)
            {
                model.Roles = await GetRoleListAsync();
                TempData["Error"] = "Please fix the validation errors.";
                return View("Index", model);
            }

            if (await _roleManager.RoleExistsAsync(model.NewRole.RoleName.Trim()))
            {
                TempData["Error"] = "This role already exists!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _roleManager.CreateAsync(new IdentityRole(model.NewRole.RoleName.Trim()));
            if (result.Succeeded)
            {
                TempData["Success"] = "Role created successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Failed to create role.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var model = new RoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name!
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            ModelState.Remove(nameof(model.RoleId));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(model.RoleId!);
            if (role == null) return NotFound();

            role.Name = model.RoleName.Trim();
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["Success"] = "Role updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string roleId)
        {
            if (string.IsNullOrEmpty(roleId)) return NotFound();

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["Success"] = "Role deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to delete role.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}