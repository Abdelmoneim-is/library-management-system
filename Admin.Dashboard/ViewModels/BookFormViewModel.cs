using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.ViewModels
{
    public class BookFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(50)]
        public string ISBN { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Display(Name = "Published Year")]
        [Range(1000, 2100)]
        public int PublishedYear { get; set; }

        [Required(ErrorMessage = "Publisher is required.")]
        [StringLength(100)]
        public string Publisher { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language is required.")]
        [StringLength(50)]
        public string Language { get; set; } = string.Empty;

        [Display(Name = "Page Count")]
        [Range(1, 10000)]
        public int PageCount { get; set; }

        [Display(Name = "Total Copies")]
        [Range(1, 1000)]
        public int TotalCopies { get; set; }

        [Display(Name = "Cover Image URL")]
        public string? CoverImageURL { get; set; }

        [Required(ErrorMessage = "Author is required.")]
        [Display(Name = "Author")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Authors { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
    }
}
