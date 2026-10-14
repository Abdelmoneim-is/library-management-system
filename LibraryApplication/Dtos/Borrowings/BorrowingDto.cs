using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Borrowings
{
    public class BorrowingDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }
        public bool IsOverDue { get; set; }

        public int ExtensionCount { get; set; }
        public decimal FineAmount { get; set; }
        public bool IsFinePaid { get; set; }
        public DateTime? FinePaidAt { get; set; }
        public bool HasUnpaidFine { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
    }
}
