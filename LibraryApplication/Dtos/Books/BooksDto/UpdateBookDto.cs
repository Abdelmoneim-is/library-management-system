using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Books.BooksDto
{
    public class UpdateBookDto
    {
        [Required(ErrorMessage = "Book title is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Book title must be between 2 and 200 characters.")]
        public string Title { get; set; } = default!;

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(20, ErrorMessage = "ISBN must not exceed 20 characters.")]
        public string ISBN { get; set; } = default!;

        [StringLength(1000, ErrorMessage = "Description must not exceed 1000 characters.")]
        public string? Description { get; set; }

        [Range(1000, 2100, ErrorMessage = "Published year is invalid.")]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = "Publisher is required.")]
        [StringLength(100, ErrorMessage = "Publisher name must not exceed 100 characters.")]
        public string Publisher { get; set; } = default!;

        [Required(ErrorMessage = "Language is required.")]
        [StringLength(50, ErrorMessage = "Language must not exceed 50 characters.")]
        public string Language { get; set; } = default!;

        [Range(1, 10000, ErrorMessage = "Page count must be greater than 0.")]
        public int PageCount { get; set; }

        [Range(1, 1000, ErrorMessage = "Total copies must be at least 1.")]
        public int TotalCopies { get; set; }

        public string? CoverImageURL { get; set; }

        [Required(ErrorMessage = "Author ID is required.")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }
    }
}
