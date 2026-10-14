using LibraryApplication.Dtos.Identity;
using LibraryInfrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Dashboard.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AdminController(UserManager<AppUser> userManager , SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid Login Attemp");
                return View(loginDto);
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
            if (!result.Succeeded || (!await _userManager.IsInRoleAsync(user, "Admin") && (!await _userManager.IsInRoleAsync(user, "SuperVisor"))))
            {
                ModelState.AddModelError("", "You Are Not Authorized");
                return View(loginDto);
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
