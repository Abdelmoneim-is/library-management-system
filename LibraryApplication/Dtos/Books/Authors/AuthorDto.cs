using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Books.Authors
{
    public class AuthorDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = default!;

        public string? Bio { get; set; }

        public string? Nationality { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
