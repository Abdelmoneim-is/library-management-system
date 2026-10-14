using LibraryApplication.Common.BorrowingsQueryParams;
using LibraryDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal class BorrowingsAdminCount : BaseSpecification<Borrowing, int>
    {
        public BorrowingsAdminCount(BorrowingsQueryParams queryParams)
           : base(BorrowingFilters.Build(queryParams)) { }
    }
}
