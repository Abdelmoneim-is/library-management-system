using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class UnpaidFinesWithBookSpec : BaseSpecification<Borrowing , int>
    {
        public UnpaidFinesWithBookSpec()
           : base(b => b.FineAmount > 0 && !b.IsFinePaid)
        {
            AddInclude(b => b.Book);
            AddOrder(b => b.DueDate);
        }
    }
}
