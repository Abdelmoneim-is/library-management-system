using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Books.Categories
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "This Name Is Required")]
        [MaxLength(150, ErrorMessage =  "The category name must not exceed 150 characters")]
        public string Name { get; set; } = default!;
        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
