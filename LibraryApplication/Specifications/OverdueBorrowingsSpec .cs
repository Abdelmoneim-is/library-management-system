using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class OverdueBorrowingsSpec : BaseSpecification<Borrowing, int>
    {
        public OverdueBorrowingsSpec()
           : base(b => b.ReturnDate == null && b.DueDate < DateTime.UtcNow)
        {
            AddInclude(b => b.Book);
            AddOrder(b => b.DueDate);   
        }
    }
}
