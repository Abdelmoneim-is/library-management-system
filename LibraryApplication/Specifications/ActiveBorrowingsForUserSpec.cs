using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class ActiveBorrowingsForUserSpec : BaseSpecification<Borrowing , int>
    {
        public ActiveBorrowingsForUserSpec(string userId)
            : base(b => b.UserId == userId && b.ReturnDate == null)
        {
        }
    }
}
