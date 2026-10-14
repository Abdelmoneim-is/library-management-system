using LibraryDomain.Common;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BorrowingWithBookSpecification : BaseSpecification<Borrowing , int>
    {
        public BorrowingWithBookSpecification(string userId) : base (b => b.UserId == userId)
        {
            AddInclude(b => b.Book);
            AddOrderByDescinding(b => b.BorrowDate);
        }
        public BorrowingWithBookSpecification() : base (b => true)
        {
            AddInclude(b => b.Book);
            AddOrderByDescinding (b => b.BorrowDate);
        }
        public BorrowingWithBookSpecification(int id) : base(b => b.Id == id)
        {
            AddInclude(b => b.Book);
        }

        public BorrowingWithBookSpecification(string userId , int bookId) : base (b => b.UserId == userId && b.BookId == bookId)
        {
            
        }

    }
}
