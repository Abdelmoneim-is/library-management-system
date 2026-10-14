using LibraryDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Entities
{
    public class BookAuthor : BaseEntity<int>
    {
        public string FullName { get; set; } = default!;
        public string? Bio { get; set; }
        public string? Nationality { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
