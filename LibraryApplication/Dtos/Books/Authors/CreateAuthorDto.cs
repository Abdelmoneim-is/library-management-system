using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Books.Authors
{
    public class CreateAuthorDto
    {
        [Required(ErrorMessage = "Author Name Is Required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Author Name Must Between 2 and 15 ")]
        public string FullName { get; set; } = default!;

        [StringLength(1000, ErrorMessage = "The biography must not exceed 1,000 characters")]
        public string? Bio { get; set; }

        [StringLength(100, ErrorMessage = "The nationality must not exceed 100 characters.")]
        public string? Nationality { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}
