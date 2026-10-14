using Admin.Dashboard.ViewModels;
using LibraryDomain.Entities;
using LibraryInfrastructure.Data;
using LibraryInfrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Admin.Dashboard.Controllers
{
    [Route("Borrowings/[action]")]
    public class BorrowingsController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BorrowingsController(StoreDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("Borrowings/Index")]
        public async Task<IActionResult> Index(string? searchQuery, string? filterStatus)
        {
            var query = _context.Borrowings
                .Include(l => l.Book)
                .AsQueryable();

            // 1. الفلترة حسب الحالة (Status Filter)
            if (!string.IsNullOrEmpty(filterStatus))
            {
                if (filterStatus == "active")
                {
                    query = query.Where(l => l.ReturnDate == null && DateTime.UtcNow <= l.DueDate);
                }
                else if (filterStatus == "overdue")
                {
                    query = query.Where(l => l.ReturnDate == null && DateTime.UtcNow > l.DueDate);
                }
                else if (filterStatus == "returned")
                {
                    query = query.Where(l => l.ReturnDate != null);
                }
                else if (filterStatus == "unpaid-fines")
                {
                    query = query.Where(l => l.FineAmount > 0 && !l.IsFinePaid);
                }
                else if (filterStatus == "paid-fines")
                {
                    query = query.Where(l => l.FineAmount > 0 && l.IsFinePaid);
                }
            }

            // 2. البحث (Search Query: باسم الكتاب أو معرف المستخدم UserId)
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(l => (l.Book != null && l.Book.Title.Contains(searchQuery)) || l.UserId.Contains(searchQuery));
            }

            var loans = await query
                .OrderByDescending(l => l.BorrowDate)
                .Select(l => new LoanViewModel
                {
                    Id = l.Id,
                    BookTitle = l.Book != null ? l.Book.Title : "Unknown Book",
                    UserId = l.UserId,
                    BorrowDate = l.BorrowDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    FineAmount = l.FineAmount,
                    IsFinePaid = l.IsFinePaid,
                    IsReturned = l.IsReturned,
                    IsOverDue = l.IsOverDue
                })
                .ToListAsync();

            ViewData["CurrentSearch"] = searchQuery;
            ViewData["CurrentFilter"] = filterStatus;

            return View(loans);
        }

        [HttpGet]
        [Route("Borrowings/Overdue")]
        public async Task<IActionResult> Overdue()
        {
            var overdueLoans = await _context.Borrowings
                .Include(l => l.Book)
                .Where(l => l.ReturnDate == null && DateTime.UtcNow > l.DueDate)
                .Select(l => new LoanViewModel
                {
                    Id = l.Id,
                    BookTitle = l.Book != null ? l.Book.Title : "Unknown Book",
                    UserId = l.UserId,
                    BorrowDate = l.BorrowDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    FineAmount = l.FineAmount,
                    IsFinePaid = l.IsFinePaid,
                    IsReturned = l.IsReturned,
                    IsOverDue = true
                })
                .ToListAsync();

            return View(overdueLoans);
        }

        [HttpGet]
        [Route("Borrowings/UnpaidFines")]
        [Route("Borrowings/unpaid-fines")]
        public async Task<IActionResult> UnpaidFines()
        {
            var unpaidFineLoans = await _context.Borrowings
                .Include(l => l.Book)
                .Where(l => l.FineAmount > 0 && !l.IsFinePaid)
                .Select(l => new LoanViewModel
                {
                    Id = l.Id,
                    BookTitle = l.Book != null ? l.Book.Title : "Unknown Book",
                    UserId = l.UserId,
                    BorrowDate = l.BorrowDate,
                    DueDate = l.DueDate,
                    ReturnDate = l.ReturnDate,
                    FineAmount = l.FineAmount,
                    IsFinePaid = l.IsFinePaid,
                    IsReturned = l.IsReturned,
                    IsOverDue = l.IsOverDue
                })
                .ToListAsync();

            return View(unpaidFineLoans);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Loans/PayFine/{id}")]
        public async Task<IActionResult> PayFine(int id)
        {
            var borrowing = await _context.Borrowings.FindAsync(id);
            if (borrowing == null)
            {
                TempData["Error"] = "Borrowing record not found.";
                return RedirectToAction(nameof(UnpaidFines));
            }

            borrowing.IsFinePaid = true;
            borrowing.FinePaidAt = DateTime.UtcNow;

            _context.Update(borrowing);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Fine has been successfully paid by admin.";
            return RedirectToAction(nameof(UnpaidFines));
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var users = await _userManager.Users.ToListAsync();
            var books = await _context.Books.ToListAsync();

            var viewModel = new LoanFormViewModel
            {
                BorrowDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(14),
                Users = users.Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.UserName} ({u.Email})"
                }),
                Books = books.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.Title
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoanFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var users = await _userManager.Users.ToListAsync();
                var books = await _context.Books.ToListAsync();

                model.Users = users.Select(u => new SelectListItem { Value = u.Id, Text = $"{u.UserName} ({u.Email})" });
                model.Books = books.Select(b => new SelectListItem { Value = b.Id.ToString(), Text = b.Title });

                return View(model);
            }

            var loan = new Borrowing
            {
                UserId = model.UserId,
                BookId = model.BookId,
                BorrowDate = model.BorrowDate,
                DueDate = model.DueDate,
                CreatedAt = DateTime.UtcNow
            };

            _context.Borrowings.Add(loan);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Loan created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SimulateFine(int id)
        {
            var loan = await _context.Borrowings.FindAsync(id);
            if (loan == null)
            {
                TempData["Error"] = "Loan not found.";
                return RedirectToAction(nameof(Index));
            }

            loan.DueDate = DateTime.UtcNow.AddDays(-10);
            loan.FineAmount = 50.00m;
            loan.IsFinePaid = false;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Simulated overdue and fine applied successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowing == null)
            {
                TempData["Error"] = "Borrowing record not found.";
                return RedirectToAction(nameof(Index));
            }

            if (borrowing.IsReturned)
            {
                TempData["Error"] = "This book has already been returned.";
                return RedirectToAction(nameof(Index));
            }

            borrowing.ReturnDate = DateTime.UtcNow;

            if (borrowing.Book != null)
            {
                borrowing.Book.AvailableCopies += 1;
            }

            if (borrowing.ReturnDate > borrowing.DueDate)
            {
                var overdueDays = (borrowing.ReturnDate.Value - borrowing.DueDate).Days;
                if (overdueDays > 0)
                {
                    decimal dailyFineRate = 5.00m;
                    borrowing.FineAmount = overdueDays * dailyFineRate;
                    borrowing.IsFinePaid = false;

                    TempData["Error"] = $"Book returned LATE by {overdueDays} days! Fine generated: {borrowing.FineAmount:C}";
                }
            }
            else
            {
                TempData["Success"] = "Book returned successfully on time.";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}