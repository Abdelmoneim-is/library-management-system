using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Identity
{
    public class RolesDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Role { get; set; } = default!;
    }
}
