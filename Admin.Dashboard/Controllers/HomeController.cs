using Admin.Dashboard.ViewModels;
using LibraryInfrastructure.Data;
using LibraryInfrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Admin.Dashboard.Controllers
{

    public class HomeController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HomeController(StoreDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            // 1. ?????????? ????????
            var totalBooks = await _context.Books.CountAsync();
            var availableBooks = await _context.Books.CountAsync(b => b.AvailableCopies > 0);
            var totalCategories = await _context.BookCategories.CountAsync();
            var totalAuthors = await _context.BookAuthors.CountAsync();
            var totalUsers = await _userManager.Users.CountAsync();

            // 2. ???????? ?????????
            var allBorrowings = await _context.Borrowings
                .Include(b => b.Book)
                .ToListAsync();

            var activeBorrowingsCount = allBorrowings.Count(b => !b.IsReturned);
            var overdueBorrowingsList = allBorrowings.Where(b => b.IsOverDue).ToList();

            var totalUnpaidFines = allBorrowings.Where(b => b.HasUnpaidFine).Sum(b => b.FineAmount);
            var totalPaidFines = allBorrowings.Where(b => b.IsFinePaid).Sum(b => b.FineAmount);

            // ??? ????? ?????????? ???????? ????????
            var overdueViewModels = new List<OverdueBorrowingItemViewModel>();
            foreach (var item in overdueBorrowingsList.Take(5))
            {
                var user = await _userManager.FindByIdAsync(item.UserId);
                overdueViewModels.Add(new OverdueBorrowingItemViewModel
                {
                    BorrowingId = item.Id,
                    BookTitle = item.Book?.Title ?? "N/A",
                    UserName = user?.UserName ?? "Unknown User",
                    DueDate = item.DueDate,
                    OverdueDays = (int)(DateTime.UtcNow - item.DueDate).TotalDays,
                    FineAmount = item.FineAmount
                });
            }

            // 3. ???? ?????????
            var recentReviews = await _context.BookReviews
                .Include(r => r.Book)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            var recentReviewsViewModels = new List<RecentReviewItemViewModel>();
            foreach (var rev in recentReviews)
            {
                var user = await _userManager.FindByIdAsync(rev.UserId);
                recentReviewsViewModels.Add(new RecentReviewItemViewModel
                {
                    BookTitle = rev.Book?.Title ?? "N/A",
                    UserName = user?.UserName ?? "Anonymous",
                    Rating = rev.Rating,
                    Comment = rev.Comment,
                    CreatedAt = rev.CreatedAt
                });
            }

            // 4. ???????? ??????? ????? ???????
            var categoryStats = await _context.BookCategories
                .Select(c => new CategoryStatViewModel
                {
                    CategoryName = c.Name,
                    BookCount = _context.Books.Count(b => b.CategoryId == c.Id)
                }).ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalBooks = totalBooks,
                AvailableBooks = availableBooks,
                TotalCategories = totalCategories,
                TotalAuthors = totalAuthors,
                TotalUsers = totalUsers,
                ActiveBorrowings = activeBorrowingsCount,
                OverdueBorrowings = overdueBorrowingsList.Count,
                TotalUnpaidFines = totalUnpaidFines,
                TotalPaidFines = totalPaidFines,
                RecentOverdueList = overdueViewModels,
                RecentReviewsList = recentReviewsViewModels,
                CategoryStats = categoryStats
            };

            return View(viewModel);
        }
    }
}