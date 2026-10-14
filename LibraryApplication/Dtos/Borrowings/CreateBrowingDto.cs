using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Borrowings
{
    public class CreateBrowingDto
    {
        [Required]
        public int BookId { get; set; }
    }
}
