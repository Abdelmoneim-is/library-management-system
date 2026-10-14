using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class UserReviewForBookSpecification  : BaseSpecification<BookReview , int>
    {
        public UserReviewForBookSpecification(string userId , int BookId) : base (r => r.UserId == userId && r.BookId == BookId)
        {
            
        }
    }
}
