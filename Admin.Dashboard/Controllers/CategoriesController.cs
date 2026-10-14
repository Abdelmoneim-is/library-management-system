using Admin.Dashboard.ViewModels;
using LibraryDomain.Entities;
using LibraryInfrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Admin.Dashboard.Controllers
{
    [Route("admin/[controller]/[action]")]
    public class CategoriesController : Controller
    {
        private readonly StoreDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private const string CategoriesCacheKey = "CategoriesCache_Key";

        public CategoriesController(StoreDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchQuery)
        {
            if (!_memoryCache.TryGetValue(CategoriesCacheKey, out List<CategoryFormViewModel>? categories))
            {
                var query = _context.BookCategories.AsQueryable();

                categories = await query
                    .OrderByDescending(c => c.Id)
                    .Select(c => new CategoryFormViewModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    })
                    .ToListAsync();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                _memoryCache.Set(CategoriesCacheKey, categories, cacheEntryOptions);
            }

            if (!string.IsNullOrEmpty(searchQuery) && categories != null)
            {
                categories = categories.Where(c => c.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                                 (c.Description != null && c.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                                     .ToList();
            }

            ViewData["CurrentSearch"] = searchQuery;

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = new BookCategory
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = User.Identity?.Name ?? "System Admin"
            };

            _context.BookCategories.Add(category);
            await _context.SaveChangesAsync();

            _memoryCache.Remove(CategoriesCacheKey);

            TempData["Success"] = "Category created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _context.BookCategories.FindAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            var model = new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            var category = await _context.BookCategories.FindAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            category.Name = model.Name;
            category.Description = model.Description;
            category.UpdatedAt = DateTime.UtcNow;
            category.UpdatedBy = User.Identity?.Name ?? "System Admin";

            _context.BookCategories.Update(category);
            await _context.SaveChangesAsync();

            _memoryCache.Remove(CategoriesCacheKey);

            TempData["Success"] = "Category updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.BookCategories.FindAsync(id);
            if (category == null)
            {
                TempData["Error"] = "Category not found!";
                return RedirectToAction(nameof(Index));
            }

            // 🛡️ التحقق مما إذا كان هناك كتب مرتبطة بهذا القسم لمنع انهيار قاعدة البيانات
            bool hasBooks = await _context.Books.AnyAsync(b => b.CategoryId == id);
            if (hasBooks)
            {
                TempData["Error"] = "Cannot delete this category because there are books associated with it. Please delete or reassign the books first!";
                return RedirectToAction(nameof(Index));
            }

            _context.BookCategories.Remove(category);
            await _context.SaveChangesAsync();

            _memoryCache.Remove(CategoriesCacheKey);

            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}