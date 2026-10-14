using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.ViewModels
{
    public class LoanFormViewModel
    {
        [Required(ErrorMessage = "Please select a user.")]
        public string UserId { get; set; } = default!;

        [Required(ErrorMessage = "Please select a book.")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Borrow date is required.")]
        [DataType(DataType.Date)]
        public DateTime BorrowDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(14); // افتراضياً أسبوعين

        // قوائم منسدلة لعرض الكتب والمستخدمين في الـ View
        public IEnumerable<SelectListItem>? Users { get; set; }
        public IEnumerable<SelectListItem>? Books { get; set; }
    }
}
