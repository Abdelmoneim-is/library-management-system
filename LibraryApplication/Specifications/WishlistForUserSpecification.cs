using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class WishlistForUserSpecification : BaseSpecification<WishlistItem , int>
    {
        public WishlistForUserSpecification(string userId) : base (w => w.UserId == userId)
        {
            AddInclude(w => w.Book);
            AddInclude(w => w.Book.BookAuthor);
            AddInclude(w => w.Book.BookCategory);
            AddOrderByDescinding(w => w.AddedAt);
        }
    }
}
