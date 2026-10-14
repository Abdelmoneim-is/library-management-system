using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Books.BooksDto
{
    public class BookDto
    {
        public int Id { get; set; }
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

        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = default!;

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = default!;
        public double? AverageRating { get; set; }
        public int? ReviewsCount { get; set; }
    }
}
