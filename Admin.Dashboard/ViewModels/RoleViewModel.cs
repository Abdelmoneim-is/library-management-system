using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Admin.Dashboard.ViewModels
{
    public class RoleViewModel
    {
        [ValidateNever] // دي بتخلي السيرفر يتجاهل الـ RoleId وقت الإنشاء ومايعتبروش مطلوب
        public string RoleId { get; set; } = default!;

        [Required(ErrorMessage = "Role Name Is Required")]
        [StringLength(256, ErrorMessage = "Role Name Can't Be moreThan 256 Char")]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; } = default!;

        public int UserCount { get; set; }
    }
}
