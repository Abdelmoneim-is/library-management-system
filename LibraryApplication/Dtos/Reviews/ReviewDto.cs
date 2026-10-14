using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Dtos.Reviews
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
