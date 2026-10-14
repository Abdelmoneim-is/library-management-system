using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BorrowingsByBookSpecification : BaseSpecification<Borrowing , int>
    {
        public BorrowingsByBookSpecification(int bookId) : base (b => b.BookId == bookId)
        {
            
        }
    }
}
