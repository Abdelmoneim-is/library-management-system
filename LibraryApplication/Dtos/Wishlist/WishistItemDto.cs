using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Wishlist
{
    public class WishistItemDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = default!;
        public string AuthorName { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public string? CoverImageURL { get; set; }
        public int AvailableCopies { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
