using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class WishlistItemsByUserSpecification : BaseSpecification<WishlistItem , int>
    {
        public WishlistItemsByUserSpecification(string userId)
            : base(w => w.UserId == userId)
        {
        }
    }
}
