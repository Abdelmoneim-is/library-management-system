using Admin.Dashboard.ViewModels;
using LibraryInfrastructure.Data;
using LibraryInfrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Dashboard.Controllers
{

    public class ReviewsController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReviewsController(StoreDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchQuery, int? filterRating, int? bookId, string? userId)
        {
            var query = _context.BookReviews
                .Include(r => r.Book)
                .AsQueryable();

            // 1. الفلترة بكتاب معين
            if (bookId.HasValue && bookId.Value > 0)
            {
                query = query.Where(r => r.BookId == bookId.Value);
                var bookTitle = await _context.Books
                    .Where(b => b.Id == bookId.Value)
                    .Select(b => b.Title)
                    .FirstOrDefaultAsync();
                ViewBag.FilterHeader = $"Reviews for Book: {bookTitle ?? "Selected Book"}";
            }

            // 2. الفلترة بمستخدم معين
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(r => r.UserId == userId);
                ViewBag.FilterHeader = $"Reviews by User: {userId}";
            }

            // 3. الفلترة حسب التقييم بالنجوم
            if (filterRating.HasValue && filterRating.Value >= 1 && filterRating.Value <= 5)
            {
                query = query.Where(r => r.Rating == filterRating.Value);
                if (ViewBag.FilterHeader == null)
                {
                    ViewBag.FilterHeader = $"Rating: {filterRating.Value} Stars";
                }
            }

            // 4. البحث النصي (في اسم الكتاب أو محتوى التعليق)
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(r => (r.Book != null && r.Book.Title.Contains(searchQuery)) || r.Comment.Contains(searchQuery));
                if (ViewBag.FilterHeader == null)
                {
                    ViewBag.FilterHeader = $"Search Query: '{searchQuery}'";
                }
            }

            var reviews = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    BookId = r.BookId,
                    BookTitle = r.Book != null ? r.Book.Title : "Unknown Book",
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();

            // تجهيز القوائم المنسدلة للفلترة
            ViewBag.Books = await _context.Books
                .Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Title })
                .ToListAsync();

            ViewBag.Users = await _userManager.Users
                .Select(u => new SelectListItem { Value = u.Id, Text = u.UserName ?? u.Email })
                .ToListAsync();

            ViewData["CurrentSearch"] = searchQuery;
            ViewData["CurrentRating"] = filterRating;
            ViewData["CurrentBookId"] = bookId;
            ViewData["CurrentUserId"] = userId;

            return View(reviews);
        }

        // POST: Reviews/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.BookReviews.FindAsync(id);
            if (review == null)
            {
                TempData["Error"] = "Review not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.BookReviews.Remove(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Review has been deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/ByBook/5
        [HttpGet]
        public async Task<IActionResult> ByBook(int bookId)
        {
            return RedirectToAction(nameof(Index), new { bookId = bookId });
        }

        // GET: Reviews/ByUser?userId=xyz
        [HttpGet]
        public async Task<IActionResult> ByUser(string userId)
        {
            return RedirectToAction(nameof(Index), new { userId = userId });
        }

        // GET: Reviews/LowRatings
        [HttpGet]
        public async Task<IActionResult> LowRatings()
        {
            // إعادة التوجيه للـ Index مع فلترة التقييمات الضعيفة 1 و 2 نجوم
            return RedirectToAction(nameof(Index), new { filterRating = 1 });
        }
    }
}