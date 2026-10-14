using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.ViewModels
{
    public class UserCreateViewModel
    {
        [Required(ErrorMessage = "Display Name is required")]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; } = default!;

        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string UserName { get; set; } = default!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = default!;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Assign Role")]
        public string? SelectedRole { get; set; }
    }
}
