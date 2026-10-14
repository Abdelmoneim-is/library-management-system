using LibraryDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Entities
{
    public class Borrowing : BaseEntity<int>
    {
        public int BookId { get; set; }
        public Books Book { get; set; } = default!;

        public string UserId { get; set; } = default!;
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public int ExtensionCount { get; set; } = 0;

        public decimal FineAmount { get; set; } = 0;
        public bool IsFinePaid { get; set; } = false;

        public DateTime? FinePaidAt { get; set; }
        public bool HasUnpaidFine => FineAmount > 0 && !IsFinePaid;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public bool IsReturned => ReturnDate.HasValue;
        public bool IsOverDue => !IsReturned && DateTime.UtcNow > DueDate;
    }
}
