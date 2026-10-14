using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class UnpaidFinesForUserSpec : BaseSpecification<Borrowing , int>
    {
        public UnpaidFinesForUserSpec(string userId)
            : base(b => b.UserId == userId && b.FineAmount > 0 && !b.IsFinePaid) 
        {

        }
    }
}
