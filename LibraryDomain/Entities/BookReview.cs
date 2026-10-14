using LibraryDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Entities
{
    public class BookReview  : BaseEntity<int>
    {
        public int BookId { get; set; }
        public Books Book { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
