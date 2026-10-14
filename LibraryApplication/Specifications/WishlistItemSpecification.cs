using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class WishlistItemSpecification : BaseSpecification<WishlistItem , int>
    {
        public WishlistItemSpecification(string userId , int bookId)
            : base(w => w.UserId == userId && w.BookId  == bookId)
        {
            
        }
    }
}
