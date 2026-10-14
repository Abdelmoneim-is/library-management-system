using LibraryDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Entities
{
    public class Books : BaseEntity<int>
    {
        public string Title { get; set; } = default!;
        public string ISBN { get; set; } = default!;
        public string? Description { get; set; }
        public int PublishedYear { get; set; }
        public string Publisher { get; set; } = default!;
        public string Language { get; set; } = default!;
        public int PageCount { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string? CoverImageURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public BookAuthor BookAuthor { get; set; } = default!;
        public BookCategory BookCategory { get; set; } = default!;

        public int AuthorId { get; set; }
        public int CategoryId { get; set; }

        public byte[] RowVersion { get; set; } = default!;
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
