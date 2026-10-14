using Admin.Dashboard.ViewModels;
using LibraryInfrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Dashboard.Controllers
{
    [Route("admin/[controller]/[action]")]
    public class BooksController : Controller
    {
        private readonly StoreDbContext _context;

        public BooksController(StoreDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchQuery, int? categoryId, int? authorId, string? availability)
        {
            var query = _context.Books
                .Include(b => b.BookAuthor)
                .Include(b => b.BookCategory)
                .AsQueryable();

            // 1. فلترة حسب القسم (Category)
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            // 2. فلترة حسب المؤلف (Author)
            if (authorId.HasValue && authorId.Value > 0)
            {
                query = query.Where(b => b.AuthorId == authorId.Value);
            }

            // 3. فلترة حسب التوفر (Availability)
            if (!string.IsNullOrEmpty(availability))
            {
                if (availability == "available")
                {
                    query = query.Where(b => b.AvailableCopies > 0);
                }
                else if (availability == "out-of-stock")
                {
                    query = query.Where(b => b.AvailableCopies <= 0);
                }
            }

            // 4. البحث النصي الذكي
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(b => b.Title.Contains(searchQuery) ||
                                        (b.Description != null && b.Description.Contains(searchQuery)) ||
                                        b.ISBN.Contains(searchQuery));
            }

            var books = await query
                .OrderByDescending(b => b.Id)
                .Select(b => new BookIndexViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    ISBN = b.ISBN,
                    AuthorId = b.AuthorId,
                    CategoryId = b.CategoryId,
                    AuthorName = b.BookAuthor != null ? b.BookAuthor.FullName : "N/A",
                    CategoryName = b.BookCategory != null ? b.BookCategory.Name : "N/A",
                    AvailableCopies = b.AvailableCopies,
                    TotalCopies = b.TotalCopies
                })
                .ToListAsync();

            ViewBag.Categories = await _context.BookCategories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToListAsync();

            ViewBag.Authors = await _context.BookAuthors
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName })
                .ToListAsync();

            ViewData["CurrentSearch"] = searchQuery;
            ViewData["CurrentCategoryId"] = categoryId;
            ViewData["CurrentAuthorId"] = authorId;
            ViewData["CurrentAvailability"] = availability;

            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new BookFormViewModel
            {
                PublishedYear = DateTime.Now.Year,
                Authors = await _context.BookAuthors
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName })
                    .ToListAsync(),
                Categories = await _context.BookCategories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Authors = await _context.BookAuthors
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName })
                    .ToListAsync();
                model.Categories = await _context.BookCategories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync();
                return View(model);
            }

            var book = new LibraryDomain.Entities.Books
            {
                Title = model.Title,
                ISBN = model.ISBN,
                Description = model.Description,
                PublishedYear = model.PublishedYear,
                Publisher = model.Publisher,
                Language = model.Language,
                PageCount = model.PageCount,
                TotalCopies = model.TotalCopies,
                AvailableCopies = model.TotalCopies,
                CoverImageURL = model.CoverImageURL,
                AuthorId = model.AuthorId,
                CategoryId = model.CategoryId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

                // 🔒 تسجيل اسم المستخدم الذي قام بالإنشاء
                CreatedBy = User.Identity?.Name ?? "System Admin"
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Book created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                TempData["Error"] = "Book not found!";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new BookFormViewModel
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                PublishedYear = book.PublishedYear,
                Publisher = book.Publisher,
                Language = book.Language,
                PageCount = book.PageCount,
                TotalCopies = book.TotalCopies,
                CoverImageURL = book.CoverImageURL,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                Authors = await _context.BookAuthors
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName })
                    .ToListAsync(),
                Categories = await _context.BookCategories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync()
            };

            ViewBag.BookId = id;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookFormViewModel model)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                TempData["Error"] = "Book not found!";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Authors = await _context.BookAuthors
                    .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.FullName })
                    .ToListAsync();
                model.Categories = await _context.BookCategories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync();

                ViewBag.BookId = id;
                return View(model);
            }

            book.Title = model.Title;
            book.ISBN = model.ISBN;
            book.Description = model.Description;
            book.PublishedYear = model.PublishedYear;
            book.Publisher = model.Publisher;
            book.Language = model.Language;
            book.PageCount = model.PageCount;
            book.TotalCopies = model.TotalCopies;
            book.CoverImageURL = model.CoverImageURL;
            book.AuthorId = model.AuthorId;
            book.CategoryId = model.CategoryId;
            book.UpdatedAt = DateTime.UtcNow;

            // 🔒 تسجيل اسم المستخدم الذي قام بآخر تعديل
            book.UpdatedBy = User.Identity?.Name ?? "System Admin";

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Book updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/Books/Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                TempData["Error"] = "Book not found!";
                return RedirectToAction(nameof(Index));
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Book deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}