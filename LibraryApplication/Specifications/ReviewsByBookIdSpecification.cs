using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class ReviewsByBookIdSpecification : BaseSpecification<BookReview , int>
    {
        public ReviewsByBookIdSpecification(int bookId) : base(b => b.BookId == bookId)
        {
            AddOrderByDescinding(r => r.CreatedAt);
        }
    }
}
