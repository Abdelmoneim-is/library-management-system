using LibraryDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Entities
{
    public class WishlistItem : BaseEntity<int>
    {
        public Books Book { get; set; } = default!;
        public int BookId { get; set; }

        public string UserId { get; set; } = default!;
        public DateTime AddedAt { get; set; }
    }
}
