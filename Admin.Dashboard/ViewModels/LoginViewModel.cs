using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email or Username is required")]
        public string EmailOrUsername { get; set; } = default!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = default!;

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
