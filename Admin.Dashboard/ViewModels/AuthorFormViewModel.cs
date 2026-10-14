using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.ViewModels
{
    public class AuthorFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Author full name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string FullName { get; set; } = default!;

        public string? Bio { get; set; }
        public string? Nationality { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
