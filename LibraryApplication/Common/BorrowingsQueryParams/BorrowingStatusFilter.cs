using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Common.BorrowingsQueryParams
{
    public enum BorrowingStatusFilter
    {
        All = 0,
        Active = 1,     
        Overdue = 2,    
        Returned = 3
    }
}
