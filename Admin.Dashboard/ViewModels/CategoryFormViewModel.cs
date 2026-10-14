using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot be longer than 100 characters")]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }
    }
}
