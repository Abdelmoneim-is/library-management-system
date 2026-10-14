using Admin.Dashboard.ViewModels;
using LibraryDomain.Entities;
using LibraryInfrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Dashboard.Controllers
{
    [Route("admin/[controller]/[action]")]
    public class AuthorsController : Controller
    {
        private readonly StoreDbContext _context;

        public AuthorsController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? authorName, string? nationality)
        {
            var query = _context.BookAuthors.AsQueryable();

            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(a => a.FullName.Contains(authorName));
            }

            if (!string.IsNullOrEmpty(nationality))
            {
                query = query.Where(a => a.Nationality != null && a.Nationality.Contains(nationality));
            }

            var authors = await query
                .OrderByDescending(a => a.Id)
                .Select(a => new AuthorFormViewModel
                {
                    Id = a.Id,
                    FullName = a.FullName,
                    Nationality = a.Nationality,
                    Bio = a.Bio
                }).ToListAsync();

            ViewData["CurrentAuthorName"] = authorName;
            ViewData["CurrentNationality"] = nationality;

            return View(authors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var author = new BookAuthor
            {
                FullName = model.FullName,
                Bio = model.Bio,
                Nationality = model.Nationality,
                DateOfBirth = model.DateOfBirth,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

                // 🔒 تتبع من قام بإنشاء المؤلف
                CreatedBy = User.Identity?.Name ?? "System Admin"
            };

            _context.BookAuthors.Add(author);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Author created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _context.BookAuthors.FindAsync(id);
            if (author == null)
            {
                TempData["Error"] = "Author not found!";
                return RedirectToAction(nameof(Index));
            }

            var model = new AuthorFormViewModel
            {
                Id = author.Id,
                FullName = author.FullName,
                Bio = author.Bio,
                Nationality = author.Nationality,
                DateOfBirth = author.DateOfBirth
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuthorFormViewModel model)
        {
            var author = await _context.BookAuthors.FindAsync(id);
            if (author == null)
            {
                TempData["Error"] = "Author not found!";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            author.FullName = model.FullName;
            author.Bio = model.Bio;
            author.Nationality = model.Nationality;
            author.DateOfBirth = model.DateOfBirth;
            author.UpdatedAt = DateTime.UtcNow;

            // 🔒 تتبع من قام بآخر تعديل للمؤلف
            author.UpdatedBy = User.Identity?.Name ?? "System Admin";

            _context.BookAuthors.Update(author);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Author updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/Authors/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.BookAuthors.FindAsync(id);
            if (author == null)
            {
                TempData["Error"] = "Author not found!";
                return RedirectToAction(nameof(Index));
            }

            // 🛡️ التحقق مما إذا كان هناك كتب مرتبطة بهذا المؤلف لمنع حدوث SqlException (Constraint Violation)
            bool hasBooks = await _context.Books.AnyAsync(b => b.AuthorId == id);
            if (hasBooks)
            {
                TempData["Error"] = "Cannot delete this author because there are books associated with them. Please delete or reassign the books first!";
                return RedirectToAction(nameof(Index));
            }

            _context.BookAuthors.Remove(author);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Author deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}